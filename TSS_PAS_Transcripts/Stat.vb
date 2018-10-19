Imports System.IO
Imports Szunyi.BLs
Imports Szunyi.Features
Imports TSS_PAS_Transcripts.Extensions

Namespace Stat
    Public Enum Distributions
        Poisson = 1
        PolyaAeppli = 2
        Mixed_Poisson_PolyaAeppli = 3

    End Enum
    Public Class Manager
        Public Property Site As Sites
        Public Property Local_width As Integer
        Public Property WindowSize As Integer
        Public Property seq As Bio.ISequence
        Public Property Type As String
        Public Property File As System.IO.FileInfo
        Public Property Enums As New List(Of Integer)
        Public Property P_Threshold As Double
        Public Property Sort As Locations_By
        Public Property alpha As Double
        Public Property Interesting_Locations As New List(Of Basic_Location)

        Public Property Result As List(Of Distribution_Result)
        Public Sub New(Site As Sites)
            Me.Site = Site
        End Sub
        Public Function Get_Header() As String
            Dim str As New System.Text.StringBuilder
            str.Append(Szunyi.Sam.Import.Headers.Get_Comments(File)).AppendLine()
            str.Append("# Local Width:").Append(Local_width).AppendLine()
            str.Append("# Width:").Append(WindowSize).AppendLine()
            str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()
            If IsNothing(File) = False Then str.Append("#File:").Append(File.FullName).AppendLine()
            str.Append("#Type:").Append(Me.Type).AppendLine()
            Dim h = Split("p-value calculation type,mean,sd,variance,lambda,rho,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            str.Append(Szunyi.Common.Text.General.GetText(h, vbTab)).Append(vbTab)
            str.Append(Get_Distribution_Header(Me.Local_width))
            Return str.ToString
        End Function

        Public Function Add_Features(Seq As Bio.ISequence) As List(Of Bio.IO.GenBank.FeatureItem)
            Dim Key = Me.Type
            Dim Count As Integer = 0
            Dim Feats As New List(Of Bio.IO.GenBank.FeatureItem)
            If Me.Type = "PAS" Then
                For Each Item In Me.Result
                    '  Dim Key = Item.Passed.ToString.Substring(0) & Item.Is_Local_Maximum.ToString.Substring(0)
                    If Item.Is_Local_Maximum = True Then
                        Dim Loci As Bio.IO.GenBank.ILocation
                        If Item.IsComplementer = True Then
                            Loci = Szunyi.BLs.Location.Common.GetLocation(Item.Index + 1, Item.IsComplementer)

                        Else
                            Loci = Szunyi.BLs.Location.Common.GetLocation(Item.Index - 1, Item.IsComplementer)
                        End If


                        Dim Feat As New Bio.IO.GenBank.FeatureItem(Key, Loci)
                        Feat.Label = Item.Count
                        Feats.Add(Feat)

                        GenBankMetaDataManipulation.AddFeature(Seq, Feat)
                        Count += 1
                    End If
                Next
            Else ' TSS
                For Each Item In Me.Result

                    If Item.Is_Local_Maximum = True AndAlso Item.Passed = True Then
                        Dim loci = Szunyi.BLs.Location.Common.GetLocation(Item.Index, Item.IsComplementer)

                        Dim Feat As New Bio.IO.GenBank.FeatureItem(Key, loci)
                        Feat.Label = Item.Count
                        Feats.Add(Feat)

                        GenBankMetaDataManipulation.AddFeature(Seq, Feat)
                        Count += 1
                    End If
                Next
            End If
            Return Feats


        End Function
        Public Sub New(Local_width As Integer,
                                             WindowSize As Integer,
                                             Sort As Locations_By,
                                             seq As Bio.ISequence,
                                             Type As String,
                                             BLs As List(Of Basic_Location),
                                             Enums As List(Of Integer),
                                             P_Threshold As Double)
            Me.Local_width = Local_width
            Me.Sort = Sort
            Me.seq = seq
            Me.Type = Type
            Me.Sort = Sort

            Me.Enums = Enums
            Me.P_Threshold = P_Threshold
            Me.WindowSize = WindowSize

            Dim Merged = Szunyi.BLs.Merging.MergeLocations(BLs, Local_width, Sort, 0) ' As List(Of List(Of Szunyi.Location.Basic_Location))
            Dim For_Test As New List(Of Basic_Location)
            For Each M In Merged
                If M.Count > 1 Then
                    Interesting_Locations.Add(Szunyi.BLs.Filter.Get_Most_Abundants(M, Sort).First)
                End If
            Next
            Me.Site = New Sites(seq, BLs, Sort) ' .PacBio.Pacbio_Transcript_Shared.Get_Sites(For_Test, seq, Sort)
            Me.Site.Local_Width = Me.Local_width
            Me.Site.Width = Me.WindowSize
            Me.Site.p_Threshold = Me.P_Threshold
        End Sub
        Public Sub New(Local_width As Integer,
                                             WindowSize As Integer,
                                             Sort As Locations_By,
                                             seq As Bio.ISequence,
                                             Type As String,
                                             BLs As List(Of Basic_Location),
                                             The_Enum As Integer,
                                             P_Threshold As Double)
            Me.Local_width = Local_width
            Me.Sort = Sort
            Me.seq = seq
            Me.Type = Type
            Me.Sort = Sort

            Me.Enums.Add(The_Enum)
            Me.P_Threshold = P_Threshold
            Me.WindowSize = WindowSize

            Dim Merged = Szunyi.BLs.Merging.MergeLocations(BLs, Local_width, Sort, 0) ' As List(Of List(Of Szunyi.Location.Basic_Location))

            Dim For_Test As New List(Of Basic_Location)
            For Each M In Merged
                If M.Count > 1 Then
                    Interesting_Locations.Add(M.First)
                End If
            Next

            Me.Site = New Szunyi.BLs.Sites(seq, BLs, Sort) ' .PacBio.Pacbio_Transcript_Shared.Get_Sites(For_Test, seq, Sort)
            Me.Site.Local_Width = Me.Local_width
            Me.Site.Width = Me.WindowSize
            Me.Site.p_Threshold = Me.P_Threshold
        End Sub
        Private Sub Set_Interesting_Locationsq()

        End Sub
        Public Sub New(Local_width As Integer,
                                             WindowSize As Integer,
                                             Sort As Locations_By,
                                             seq As Bio.ISequence,
                                             Type As String,
                                             File As System.IO.FileInfo,
                                             Enums As List(Of Integer),
                                             P_Threshold As Double)
            Me.Local_width = Local_width
            Me.Sort = Sort
            Me.seq = seq
            Me.Type = Type
            Me.Sort = Sort
            Me.File = File
            Me.Enums = Enums
            Me.P_Threshold = P_Threshold
            Me.WindowSize = WindowSize
            Dim SAMs = Szunyi.Sam.Import.ParseAll(File)
            Dim Locis = Szunyi.Sam.Convert.To_GenBank_Locations(SAMs)
            Dim BL_LOcis = Szunyi.BLs.Convert.From_Bio_Locations(Locis)
            Dim Merged = Szunyi.BLs.Merging.MergeLocations(BL_LOcis, Local_width, Sort, 1) ' As List(Of List(Of Szunyi.Location.Basic_Location))
            Dim For_Test As New List(Of Basic_Location)
            For Each M In Merged
                If M.Count > 1 Then
                    Interesting_Locations.Add(Szunyi.BLs.Filter.Get_Most_Abundants(M, Sort).First)
                End If
            Next
            Me.Site = New Szunyi.BLs.Sites(seq, BL_LOcis, Sort) ' .PacBio.Pacbio_Transcript_Shared.Get_Sites(For_Test, seq, Sort)
            Me.Site.Local_Width = Me.Local_width
            Me.Site.Width = Me.WindowSize
            Me.Site.p_Threshold = Me.P_Threshold
        End Sub
        Public Sub New(Local_width As Integer,
                                             WindowSize As Integer,
                                             Sort As Locations_By,
                                             seq As Bio.ISequence,
                                             Type As String,
                                             Sams As List(Of Bio.IO.SAM.SAMAlignedSequence),
                                             Enums As List(Of Integer),
                                             P_Threshold As Double)
            Me.Local_width = Local_width
            Me.Sort = Sort
            Me.seq = seq
            Me.Type = Type
            Me.Sort = Sort
            Me.File = File
            Me.Enums = Enums
            Me.P_Threshold = P_Threshold
            Me.WindowSize = WindowSize

            Dim Locis = Szunyi.Sam.Convert.To_GenBank_Locations(Sams)
            Dim BL_LOcis = Szunyi.BLs.Convert.From_Bio_Locations(Locis)
            Dim Merged = Szunyi.BLs.Merging.MergeLocations(BL_LOcis, Local_width, Sort, 0) ' As List(Of List(Of Szunyi.Location.Basic_Location))
            Dim For_Test As New List(Of Basic_Location)
            For Each M In Merged
                If M.Count > 1 Then
                    Interesting_Locations.Add(Szunyi.BLs.Filter.Get_Most_Abundants(M, Sort).First)
                End If
            Next
            Me.Site = New Szunyi.BLs.Sites(seq, BL_LOcis, Sort)
            ' .PacBio.Pacbio_Transcript_Shared.Get_Sites(For_Test, seq, Sort)
            Me.Site.Local_Width = Me.Local_width
            Me.Site.Width = Me.WindowSize
            Me.Site.p_Threshold = Me.P_Threshold
        End Sub
        Private Function Get_Count(Feat As Basic_Location, Index As Integer) As Integer
            If Feat.Location.IsComplementer = True Then
                Return Me.Site.Neg(Index)
            Else
                Return Me.Site.Pos(Index)
            End If
        End Function
        Public Function Calculate() As List(Of Distribution_Result)
            Dim LocalMaxPos = Site.Get_Local_Maximums(False)
            Dim LocalMaxNeg = Site.Get_Local_Maximums(True)

            Dim Alpha = P_Threshold / (Me.WindowSize * 2 + 1) / (LocalMaxPos.Count + LocalMaxNeg.Count)
            Dim out As New List(Of Distribution_Result)
            '        Dim REngine = RDotNet.REngine.GetInstance
            '      REngine.Initialize()
            Dim jj = From x In Me.Interesting_Locations Where x.Location.TSS = 4181
            For Each E In Me.Enums
                For Each feat In Me.Interesting_Locations
                    Dim Index = feat.Location.Current_Position(Me.Site.sort)
                    Dim Count = Get_Count(feat, Index)
                    If Count > 0 Then
                        Dim x1 As New Stat.Distribution_Result(Me.Site, Me.WindowSize, Me.Local_width, Index, feat.Location.IsComplementer, Count, File, Nothing, E)

                        x1.Is_Local_Maximum = Me.Site.Is_Local_Maximum(Index, Me.WindowSize, feat.Location.IsComplementer)

                        If x1.p_value < Alpha Then
                            x1.Passed = True
                        Else
                            x1.Passed = False
                        End If
                        out.Add(x1)
                        x1.Distribution = Me.Site.Get_Distribution(feat, Me.Sort)

                    End If
                Next
            Next
            Me.Result = out
            Return out
        End Function
        Public Function Calculate(Feats As List(Of Basic_Location), LocalWidth As Integer, Width As Integer, File As FileInfo, E As Integer, Optional P_Threshold As Double = 0.05) As List(Of Distribution_Result)
            Dim LocalMaxPos = Site.Get_Local_Maximums(False)
            Dim LocalMaxNeg = Site.Get_Local_Maximums(True)

            Dim Alpha = P_Threshold / (Width * 2 + 1) / (LocalMaxPos.Count + LocalMaxNeg.Count)
            Dim out As New List(Of Distribution_Result)

            For Each feat In Feats
                Dim Index = feat.Location.Current_Position(Me.Site.sort)
                Dim Count = Get_Count(feat, Index)
                Dim x1 As New Stat.Distribution_Result(Me.Site, Width, LocalWidth, Index, feat.Location.IsComplementer, Count, File, Nothing, E)

                x1.Is_Local_Maximum = Me.Site.Is_Local_Maximum(Index, LocalWidth, feat.Location.IsComplementer)

                If x1.p_value < Alpha Then
                    x1.Passed = True
                Else
                    x1.Passed = False
                End If
                out.Add(x1)
            Next


            Return out
        End Function
        Public Function Get_Text()
            'p-value calculation type,sd,variance,lambda,rho,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            Dim str As New System.Text.StringBuilder
            For Each DR In Me.Result
                str.AppendLine()
                str.Append(DR.Name).Append(vbTab)
                str.Append(DR.mean).Append(vbTab)
                str.Append(DR.SD).Append(vbTab)
                str.Append(DR.Variance).Append(vbTab)
                str.Append(DR.lambda).Append(vbTab)
                str.Append(DR.rho).Append(vbTab)
                str.Append(DR.Index).Append(vbTab)
                str.Append(Szunyi.BLs.Location.Common.Get_Strand(DR.IsComplementer)).Append(vbTab)
                str.Append(DR.Count).Append(vbTab)
                str.Append(DR.Hundreds.Sum).Append(vbTab)
                str.Append(DR.p_value).Append(vbTab)
                str.Append(DR.Is_Local_Maximum).Append(vbTab)
                str.Append(DR.Passed).Append(vbTab)
                str.Append(Szunyi.Common.Text.General.GetText(DR.Distribution, vbTab))
            Next
            Return str.ToString

        End Function
        Public Shared Function Get_Distribution_Header(Local_Width As Integer) As String
            Dim str As New System.Text.StringBuilder
            For i1 = -Local_Width To Local_Width
                str.Append(i1).Append(vbTab)
            Next
            If str.Length > 0 Then str.Length -= 1
            Return str.ToString
        End Function
        Public Shared Function Get_Text(DRs As List(Of Distribution_Result)) As String
            Dim str As New System.Text.StringBuilder
            Dim h = Split("p-value calculation type,lambda,Variance,SD,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            str.Append(Szunyi.Common.Text.General.GetText(h, vbTab)).Append(vbTab)
            str.Append(Get_Distribution_Header(DRs.First.LocalWidth))
            For Each DR In DRs
                str.AppendLine()
                str.Append(DR.Name).Append(vbTab)
                str.Append(DR.mean).Append(vbTab)
                str.Append(DR.lambda).Append(vbTab)
                str.Append(DR.Variance).Append(vbTab)
                str.Append(DR.SD).Append(vbTab)
                str.Append(DR.rho).Append(vbTab)
                str.Append(DR.Index).Append(vbTab)
                str.Append(Szunyi.BLs.Location.Common.Get_Strand(DR.IsComplementer)).Append(vbTab)
                str.Append(DR.Count).Append(vbTab)
                str.Append(DR.Hundreds.Sum).Append(vbTab)
                str.Append(DR.p_value).Append(vbTab)
                str.Append(DR.Is_Local_Maximum).Append(vbTab)
                str.Append(DR.Passed).Append(vbTab)
                str.Append(Szunyi.Common.Text.General.GetText(DR.Distribution, vbTab))
            Next
            Return str.ToString
        End Function

        Public Sub Save(File As FileInfo, Seq As Bio.ISequence)
            Dim t = Me.Get_Header & Me.Get_Text

            Dim nFIle = Szunyi.IO.Rename.Append_Before_Extension_wNew_Extension(File, ".tsv")

            Szunyi.IO.Export.Text(t, nFIle)

            Dim t1 = Me.Site.Get_All_Count
            Dim nFIle2 = Szunyi.IO.Rename.Append_Before_Extension_wNew_Extension(File, "_Counts.tsv")

            Szunyi.IO.Export.Text(t1, nFIle2)


        End Sub
    End Class

    Public Class Distribution_Result
        Public Property File As FileInfo
        Public Property Width As Integer
        Public Property LocalWidth As Integer
        Public Property Count As Integer
        Public Property Index As Integer
        Public Property IsComplementer As Boolean
        Public Property Passed As Boolean
        Public Property Is_Local_Maximum As Boolean
        Public Property Distribution As List(Of Integer)
        Public Property Variance As Double
        Public Property lambda As Double
        Public Property rho As Double
        Public Property SD As Double
        Public Property p_value As Double
        Public Property Name As String
        Public Property Hundreds As List(Of Integer)
        Public Property mean As Double

        Public Sub New(site As Sites, width As Integer, LocalWidth As Integer, Index As Integer, isComplementer As Boolean, Value As Integer, FIle As FileInfo, REngine As Int16, E As Stat.Distributions)
            Me.Width = width
            Me.LocalWidth = LocalWidth
            Me.Count = Value
            Me.Index = Index
            Me.File = FIle
            Me.IsComplementer = isComplementer
            Hundreds = site.Get_Distributions(Index, isComplementer, site.sort, Me.Width)

            mean = Hundreds.Sum / Hundreds.Count
            '        Hundreds = site.Get_Distributions_woIndex(Index, isComplementer, site.sort, Me.Width)

            Variance = Accord.Statistics.Measures.Variance(Hundreds.ToArray)
            SD = Accord.Statistics.Measures.StandardDeviation(Hundreds.ToArray, mean)
            Me.Name = Szunyi.Common.Util_Helpers.Get_Enum_Name(Of Stat.Distributions)(E)

            Select Case E
                Case Stat.Distributions.Poisson
                    Dim poi As New Accord.Statistics.Distributions.Univariate.PoissonDistribution(mean)
                    p_value = 1 - poi.DistributionFunction(Value, True)

            End Select


        End Sub

        Private Function R() As String
            Dim str As New System.Text.StringBuilder
            str.Append("pPolyaAeppli(").Append(Me.Count).Append(",").Append(Me.lambda).Append(",").Append(Me.rho).Append(",log = FALSE)")
            Return str.ToString
        End Function
    End Class
End Namespace
