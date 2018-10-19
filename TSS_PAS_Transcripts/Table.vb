Imports System.IO
Imports System.Text
Imports Bio
Imports Szunyi.BLs

Public Class Table
#Region "TSS"
    Public Shared Function TSS(Potential_TSS As List(Of Basic_Location),
                              Width As Integer,
                               LocalWidth As Integer,
                               P_Threshold As Double,
                               Seq As Bio.ISequence,
                               File As FileInfo,
                               BLs As List(Of Bio.IO.GenBank.ILocation),
                              Enums As List(Of Integer)) As List(Of Stat.Distribution_Result)


        Dim str As New System.Text.StringBuilder
        str.Append(Szunyi.Sam.Import.Headers.Get_Comments(File)).AppendLine()
        str.Append("# Local Width:").Append(LocalWidth).AppendLine()
        str.Append("# Width:").Append(Width).AppendLine()
        str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()
        str.Append("#File:").Append(File.FullName).AppendLine()
        Dim Potential_TSS_Iloci = (From x1 In Potential_TSS Select x1.Location).ToList
        Dim x As New Sites(Seq, BLs, Locations_By.TSS, Width, LocalWidth, P_Threshold)
        str.Append("#Alpha:").Append(x.Get_alpha).AppendLine()

        Dim h = Split("p-value calculation type,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
        str.Append(Szunyi.Common.Text.General.GetText(h, vbTab)).Append(vbTab)
        str.Append(x.Get_Distribution_Header)
        Dim Distribution = x.Get_Distribution_Values(Potential_TSS)

        Dim Res As New List(Of Stat.Distribution_Result)
        For Each E In Enums
            Dim l As New Stat.Manager(x)
            Dim k = l.Calculate(Potential_TSS, LocalWidth, Width, File, E)
            For i1 = 0 To k.Count - 1
                k(i1).Distribution = Distribution(i1)
            Next
            Res.AddRange(k)
        Next


        Return Res

    End Function

#End Region
    Public Shared Function False_PAS(Feats As List(Of List(Of Bio.IO.GenBank.ILocation)),
                               Width As Integer,
                               LocalWidth As Integer,
                               P_Threshold As Double,
                               Seqs As List(Of Bio.ISequence),
                               File As FileInfo,
                               Sams As List(Of Bio.IO.SAM.SAMAlignedSequence),
                               BLs As List(Of Bio.IO.GenBank.ILocation),
                                     Enums As List(Of Integer)) As String

        Dim str As New System.Text.StringBuilder
        str.Append(Szunyi.Sam.Import.Headers.Get_Comments(File)).AppendLine()
        str.Append("#:Local Width=").Append(LocalWidth).AppendLine()
        str.Append("#:Width=").Append(Width).AppendLine()
        str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()
        str.Append("# File:").Append(File.FullName).AppendLine()
        Dim x As New Szunyi.BLs.Sites(Seqs.First, BLs, Locations_By.PAS, Width, LocalWidth, P_Threshold)

        str.Append("#Alpha:").Append(x.Get_alpha).AppendLine()

        Dim h = Split("Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
        str.Append(Szunyi.Common.Text.General.GetText(h, vbTab)).Append(vbTab)
        str.Append(x.Get_Distribution_Header).Append(vbTab)
        ' Dim h1 = Split("Signal Sequnece,Distance From PAS,Distance from Optimal Position)", ",")
        '    str.Append(Szunyi.Common.Text.General.GetText(h1, vbTab)).Append(vbTab)
        '    str.Append(x.Get_Nof_As_Heder)
        Dim For_PAS = Szunyi.BLs.Convert.From_ListOf_ListOf_Ilocation(Feats)
        '    Dim x1 = Szunyi.DNA.PA.Get_PolyA_Signals(Seqs.First, For_PAS, 50, -22)
        '   Dim PA = Szunyi.DNA.PA.Get_Poly_Signals_ToString(x1)
        Dim PoIssons = x.Poisson(For_PAS, 10, 50, File)
        Dim Distribution = x.Get_Distribution(For_PAS)
        '   Dim nof_A = x.Get_Nof_As(For_PAS, Seqs.First)
        For i1 = 0 To PoIssons.Count - 1
            str.AppendLine()
            str.Append(PoIssons(i1)).Append(Distribution(i1)).Append(vbTab) ' .Append(PA(i1)).Append(vbTab).Append(nof_A(i1))
        Next
        Return str.ToString
    End Function
    Public Shared Function PAS(Potential_PAS As List(Of Basic_Location),
                              Width As Integer,
                               LocalWidth As Integer,
                               P_Threshold As Double,
                               Seq As Bio.ISequence,
                               File As FileInfo,
                               BLs As List(Of Bio.IO.GenBank.ILocation),
                               Enums As List(Of Integer)) As List(Of Stat.Distribution_Result)

        Dim str As New System.Text.StringBuilder
        str.Append(Szunyi.Sam.Import.Headers.Get_Comments(File)).AppendLine()
        str.Append("# Local Width:").Append(LocalWidth).AppendLine()
        str.Append("# Width:").Append(Width).AppendLine()
        str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()
        str.Append("#File:").Append(File.FullName).AppendLine()
        Dim Potential_TSS_Iloci = (From x1 In Potential_PAS Select x1.Location).ToList
        Dim x As New Szunyi.BLs.Sites(Seq, BLs, Locations_By.PAS, Width, LocalWidth, P_Threshold)
        str.Append("#Alpha:").Append(x.Get_alpha).AppendLine()

        Dim h = Split("Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
        str.Append(Szunyi.Common.Text.General.GetText(h, vbTab)).Append(vbTab)
        str.Append(x.Get_Distribution_Header)
        Dim Distribution = x.Get_Distribution_Values(Potential_PAS)

        Dim Res As New List(Of Stat.Distribution_Result)
        For Each E In Enums
            Dim l As New Stat.Manager(x)
            Dim k = l.Calculate(Potential_PAS, LocalWidth, Width, File, E)
            For i1 = 0 To k.Count - 1
                k(i1).Distribution = Distribution(i1)
            Next
            Res.AddRange(k)
        Next


        Return Res

    End Function
    Public Shared Function PAS(Feats As List(Of Bio.IO.GenBank.FeatureItem),
                               Width As Integer,
                               LocalWidth As Integer,
                               P_Threshold As Double,
                               Seqs As List(Of Bio.ISequence),
                               File As FileInfo,
                               Sams As List(Of Bio.IO.SAM.SAMAlignedSequence),
                               BLs As List(Of Bio.IO.GenBank.ILocation),
                               Enums As List(Of Integer)) As String

        Dim str As New System.Text.StringBuilder
        str.Append(Szunyi.Sam.Import.Headers.Get_Comments(File)).AppendLine()
        str.Append("#:Local Width=").Append(LocalWidth).AppendLine()
        str.Append("#:Width=").Append(Width).AppendLine()
        str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()

        Dim x As New Sites(Seqs.First, BLs, Locations_By.PAS, Width, LocalWidth, P_Threshold)
        str.Append("#Alpha:").Append(x.Get_alpha).AppendLine()

        Dim h = Split("Feature Key,Feature Label,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
        str.Append(Szunyi.Common.Text.General.GetText(h, vbTab)).Append(vbTab)
        str.Append(x.Get_Distribution_Header).Append(vbTab)
        Dim h1 = Split("Signal Sequnece,Distance From PAS,Distance from Optimal Position)", ",")
        str.Append(Szunyi.Common.Text.General.GetText(h1, vbTab)).Append(vbTab)

        '    Dim x1 = Szunyi.DNA.PA.Get_PolyA_Signals(Seqs.First, Feats, 50, -22)
        '  Dim PA = Szunyi.DNA.PA.Get_Poly_Signals_ToString(x1)
        Dim PoIssons = x.Poisson(Feats, 10, 50, File)
        Dim Distribution = x.Get_Distribution(Feats)
        For i1 = 0 To PoIssons.Count - 1
            str.AppendLine()
            str.Append(PoIssons(i1)).Append(Distribution(i1)).Append(vbTab) '.Append(PA(i1))
        Next
        Return str.ToString
    End Function
#Region "Introns"
    Public Shared Sub Intron(minIntronLength As Integer,
                             maxIntronLength As Integer,
                             MinExonBorderLength As Integer,
                             file As FileInfo,
                             Seqs As List(Of ISequence),
                             I_Or_M As Boolean,
                             wOrientation As Boolean)
        '    Dim All_Sites As New List(Of Szunyi.BAM.SAM_Manipulation.Location.MdCigar)


        Dim Real_Introns As New List(Of Bio.IO.GenBank.ILocation)
        For Each sam In Szunyi.Sam.Import.Parse(file)

            Dim Loci = Szunyi.Sam.Convert.To_GenBank_Location(sam)

            Dim Intron = Szunyi.BLs.Location.Common.Get_All_Intron_Location(Loci)
            Dim Exons = Szunyi.BLs.Location.Common.Get_All_Exon_Location(Loci)
            For i1 = 0 To Intron.Count - 1
                Dim cIntron = Intron(i1)
                Dim s = Szunyi.BLs.Location.Common.GetLocationString(cIntron)
                s = s.Replace(cIntron.LocationEnd, cIntron.LocationEnd + 1)
                cIntron = Szunyi.BLs.Location.Common.Get_Location(s)

                Dim Intron_length = Szunyi.BLs.Location.Common.Get_Length(cIntron)
                If Intron_length >= minIntronLength AndAlso Intron_length <= maxIntronLength Then
                    Dim e1_length = Szunyi.BLs.Location.Common.Get_Length(Exons(i1))
                    If e1_length >= MinExonBorderLength Then
                        Dim e2_length = Szunyi.BLs.Location.Common.Get_Length(Exons(i1 + 1))
                        If e2_length >= MinExonBorderLength Then
                            Real_Introns.Add(cIntron)
                        End If
                    End If
                End If

            Next


        Next
        If wOrientation = True Then
            Dim Introns_TDT = Get_Introns_tdt(Seqs.First, Real_Introns, minIntronLength, maxIntronLength, MinExonBorderLength, True)
            Dim NFIle As New FileInfo(file.FullName & "_Intron_Analysis_With_Orientation.tsv")
            Szunyi.IO.Export.Text(Introns_TDT, NFIle)
        Else
            Real_Introns = Szunyi.BLs.Location.Common.Set_Direction(Real_Introns, True)
            Dim Introns_TDT = Get_Introns_tdt(Seqs.First, Real_Introns, minIntronLength, maxIntronLength, MinExonBorderLength, False)
            Dim NFIle As New FileInfo(file.FullName & "_Intron_Analysis_With_Out_Orientation.tsv")
            Szunyi.IO.Export.Text(Introns_TDT, NFIle)
        End If




    End Sub

    Public Shared Sub Intron(minIntronLength As Integer,
                             maxIntronLength As Integer,
                             MinExonBorderLength As Integer,
                             files As List(Of FileInfo),
                             Seqs As List(Of ISequence),
                             I_Or_M As Boolean,
                             wOrientation As Boolean)

        For Each FIle In files

            Dim Real_Introns As New List(Of Bio.IO.GenBank.ILocation)
            For Each sam In Szunyi.Sam.Import.Parse(FIle)

                Dim Loci = Szunyi.Sam.Convert.To_GenBank_Location(sam)

                Dim Intron = Szunyi.BLs.Location.Common.Get_All_Intron_Location(Loci)
                Dim Exons = Szunyi.BLs.Location.Common.Get_All_Exon_Location(Loci)
                For i1 = 0 To Intron.Count - 1
                    Dim cIntron = Intron(i1)

                    Dim Intron_length = Szunyi.BLs.Location.Common.Get_Length(cIntron)
                    If Intron_length >= minIntronLength AndAlso Intron_length <= maxIntronLength Then
                        Dim e1_length = Szunyi.BLs.Location.Common.Get_Length(Exons(i1))
                        If e1_length >= MinExonBorderLength Then
                            Dim e2_length = Szunyi.BLs.Location.Common.Get_Length(Exons(i1 + 1))
                            If e2_length >= MinExonBorderLength Then
                                Real_Introns.Add(cIntron)
                            End If
                        End If
                    End If

                Next


            Next
            If wOrientation = True Then
                Dim Introns_TDT = Get_Introns_tdt(Seqs.First, Real_Introns, minIntronLength, maxIntronLength, MinExonBorderLength, True)
                Dim NFIle As New FileInfo(FIle.FullName & "_Intron_Analysis_With_Orientation.tsv")
                Szunyi.IO.Export.Text(Introns_TDT, NFIle)
            Else
                Real_Introns = Szunyi.BLs.Location.Common.Set_Direction(Real_Introns, True)
                Dim Introns_TDT = Get_Introns_tdt(Seqs.First, Real_Introns, minIntronLength, maxIntronLength, MinExonBorderLength, False)
                Dim NFIle As New FileInfo(FIle.FullName & "_Intron_Analysis_With_Out_Orientation.tsv")
                Szunyi.IO.Export.Text(Introns_TDT, NFIle)
            End If


        Next

    End Sub

    Public Shared Function Get_Introns_tdt(Seq As Bio.ISequence,
                                           Locis As List(Of Bio.IO.GenBank.ILocation),
                                           MinIntronLength As Integer,
                                           MaxIntronLength As Integer,
                                           MinExonBorderLength As Integer,
                                           wOrientation As Boolean) As String

        Dim SplicesSitesII As New System.Text.StringBuilder
        SplicesSitesII.Append(Szunyi.Common.Text.General.GetText(Split("Nof Read:Strand:Location Start:Location End:Intron Length:Donor Site Sequence:Acceptor Site Sequence:Donor Site +-4 bp Sequence:Acceptor Site +-4 bp Sequence:Donor Site Alignment:Acceptor Site ALignment:Consensus Sequence Of Repeat:Length of Repeat:StarOffSet Donon:StaroffSet Acceptor", ":"), vbTab)).AppendLine()

        Dim Sw As New Bio.Algorithms.Alignment.SmithWatermanAligner
        Dim x As New Bio.SimilarityMatrices.DiagonalSimilarityMatrix(4, -4)

        Sw.SimilarityMatrix = x
        Sw.GapOpenCost = -4
        Sw.GapExtensionCost = -4


        SplicesSitesII.Append("#DiagonalSimilarityMatrix:").Append(x.DiagonalValue).Append(":").Append(x.OffDiagonalValue).AppendLine()
        SplicesSitesII.Append("#GapOpenCost:").Append(Sw.GapOpenCost).AppendLine()
        SplicesSitesII.Append("#GapExtensionCost:").Append(Sw.GapExtensionCost).AppendLine()
        SplicesSitesII.Append("#MinIntronLength:").Append(MinIntronLength).AppendLine()
        SplicesSitesII.Append("#MaxIntronLength:").Append(MaxIntronLength).AppendLine()
        SplicesSitesII.Append("#MinExonBorderLength:").Append(MinExonBorderLength).AppendLine()
        Dim B_Locis = Szunyi.BLs.Convert.From_Bio_Locations(Locis)

        Dim CV = Szunyi.BLs.Merging.GroupBy(B_Locis, Locations_By.TSS & Locations_By.PAS, 1)
        Dim TSs As New List(Of TemplateSwitch)

        For Each Loci In CV
            TSs.Add(New TemplateSwitch(Seq, Loci))
            TSs.Last.Count = Loci.Count

        Next
        For Each Item In TSs
            SplicesSitesII.Append(Item.ToString).AppendLine()
        Next
        Return SplicesSitesII.ToString
    End Function

#End Region


End Class

Public Class Poisson_Result
    Public Property Poissons As List(Of Poisson)
    ' Public Property distribution As List(Of List(Of Integer))
    Public Property Header As String

    Public Sub New(poIssons As List(Of Poisson), distribution As List(Of List(Of Integer)), str As StringBuilder, file As FileInfo)
        For i1 = 0 To poIssons.Count - 1
            poIssons(i1).Distribution = distribution(i1)
        Next
        Me.Poissons = poIssons
        '      Me.distribution = distribution
        Me.Header = str.ToString
    End Sub
    Public Overrides Function ToString() As String
        Dim str As New System.Text.StringBuilder
        str.Append(Header)
        For i1 = 0 To Poissons.Count - 1
            str.AppendLine()
            str.Append(Poissons(i1).ToString)

        Next
        Return str.ToString
    End Function
End Class
