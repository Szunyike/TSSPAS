Imports Bio.IO.GenBank
Imports System.Runtime.CompilerServices
Imports Szunyi.BLs
Imports Szunyi.Sam.Alignment

Namespace Extensions

    Module Module1
        <Extension()>
        Public Function TSS(Location As Bio.IO.GenBank.Location) As Integer
            If Location.Operator = LocationOperator.Complement Then
                Return Location.LocationEnd
            ElseIf Location.Operator = LocationOperator.Join AndAlso Location.SubLocations.Count > 0 AndAlso Location.SubLocations.First.Operator = LocationOperator.Complement Then
                Return Location.LocationEnd
            Else
                Return Location.LocationStart
            End If
        End Function
        <Extension()>
        Public Function PAS(Location As Bio.IO.GenBank.Location) As Integer
            If Location.Operator = LocationOperator.Complement Then
                Return Location.LocationStart
            ElseIf Location.Operator = LocationOperator.Join AndAlso Location.SubLocations.Count > 0 AndAlso Location.SubLocations.First.Operator = LocationOperator.Complement Then
                Return Location.LocationStart
            Else
                Return Location.LocationEnd
            End If
        End Function
        <Extension()>
        Public Function IsComplementer(Location As Bio.IO.GenBank.Location) As Boolean
            If Location.Operator = LocationOperator.Complement Then
                Return True
            ElseIf Location.Operator = LocationOperator.Join AndAlso Location.SubLocations.Count > 0 AndAlso Location.SubLocations.First.Operator = LocationOperator.Complement Then
                Return True
            Else
                Return False
            End If
        End Function
        <Extension()>
        Public Function Current_Position(Location As Bio.IO.GenBank.Location, Loc_By As Locations_By) As Integer
            Select Case Loc_By
                Case Locations_By.LE
                    Return Location.LocationEnd
                Case Locations_By.LS
                    Return Location.LocationStart
                Case Locations_By.PAS
                    Return Location.PAS
                Case Locations_By.TSS
                    Return Location.TSS
                Case Else
                    Return 0
            End Select

        End Function

        <Extension()>
        Public Function TSS(Location As Bio.IO.GenBank.ILocation) As Integer
            If Location.Operator = LocationOperator.Complement Then
                Return Location.LocationEnd
            ElseIf Location.Operator = LocationOperator.Join AndAlso Location.SubLocations.Count > 0 AndAlso Location.SubLocations.First.Operator = LocationOperator.Complement Then
                Return Location.LocationEnd
            Else
                Return Location.LocationStart
            End If
        End Function
        <Extension()>
        Public Function PAS(Location As Bio.IO.GenBank.ILocation) As Integer
            If Location.Operator = LocationOperator.Complement Then
                Return Location.LocationStart
            ElseIf Location.Operator = LocationOperator.Join AndAlso Location.SubLocations.Count > 0 AndAlso Location.SubLocations.First.Operator = LocationOperator.Complement Then
                Return Location.LocationStart
            Else
                Return Location.LocationEnd
            End If
        End Function
        <Extension()>
        Public Function IsComplementer(Location As Bio.IO.GenBank.ILocation) As Boolean
            If Location.Operator = LocationOperator.Complement Then
                Return True
            ElseIf Location.Operator = LocationOperator.Join AndAlso Location.SubLocations.Count > 0 AndAlso Location.SubLocations.First.Operator = LocationOperator.Complement Then
                Return True
            Else
                Return False
            End If
        End Function
        <Extension()>
        Public Function Current_Position(Location As Bio.IO.GenBank.ILocation, Loc_By As Locations_By) As Integer
            Select Case Loc_By
                Case Locations_By.LE
                    Return Location.LocationEnd
                Case Locations_By.LS
                    Return Location.LocationStart
                Case Locations_By.PAS
                    Return Location.PAS
                Case Locations_By.TSS
                    Return Location.TSS
                Case Else
                    Return 0
            End Select
        End Function
        <Extension()>
        Public Function ConvertToString(Seq As Bio.ISequence) As String
            Return System.Text.Encoding.Default.GetString(Seq.ToArray)
        End Function

        <Extension()>
        Public Function ConvertTo_BioSequenceNA(Seq As Bio.ISequence) As Bio.Sequence
            Return New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Seq.ToArray)
        End Function

        <Extension()>
        Public Function ConvertTo_BioSequenceAA(Seq As Bio.ISequence) As Bio.Sequence
            Return New Bio.Sequence(Bio.Alphabets.AmbiguousProtein, Seq.ToArray)
        End Function
        <Extension()>
        Public Function ConvertTo_BioSequenceNA(Seq As Bio.Sequence) As Bio.Sequence
            Return New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Seq.ToArray)
        End Function

        <Extension()>
        Public Function ConvertTo_BioSequenceAA(Seq As Bio.Sequence) As Bio.Sequence
            Return New Bio.Sequence(Bio.Alphabets.AmbiguousProtein, Seq.ToArray)
        End Function
        <Extension()>
        Public Function ConvertTo_BioSequenceNA(Seq As Bio.QualitativeSequence) As Bio.Sequence
            Return New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Seq.ToArray)
        End Function

        <Extension()>
        Public Function ConvertTo_BioSequenceAA(Seq As Bio.QualitativeSequence) As Bio.Sequence
            Return New Bio.Sequence(Bio.Alphabets.AmbiguousProtein, Seq.ToArray)
        End Function

        <Extension()>
        Public Function To_BioLocation(Own_Al As Szunyi.Sam.Alignment.Own_Al) As Bio.IO.GenBank.Location
            Dim Introns = From x In Own_Al.Parts Where x.Type = Own_Al.Type.Intron
            Dim RefStart As Integer = 0
            If Own_Al.Parts.First.Type = Own_Al.Type.Soft_Clip Then
                RefStart = Own_Al.Parts(1).Ref_Start
            Else
                RefStart = Own_Al.Parts.First.Ref_Start
            End If
            Dim RefEnd As Integer = 0
            If Own_Al.Parts.Last.Type = Own_Al.Type.Soft_Clip Then
                RefEnd = Own_Al.Parts(Own_Al.Parts.Count - 2).Ref_End
            Else
                RefEnd = Own_Al.Parts.Last.Ref_End
            End If

            Dim IsReverse As Boolean = False
            If Own_Al.Sam.Flag = Bio.IO.SAM.SAMFlags.QueryOnReverseStrand Then
                IsReverse = True
            End If
            Dim lBuilder As New Bio.IO.GenBank.LocationBuilder
            If Introns.Count = 0 Then
                Dim l As ILocation
                If IsReverse = True Then
                    l = lBuilder.GetLocation("complement(" & RefStart & ".." & RefEnd & ")")
                Else
                    l = lBuilder.GetLocation(RefStart & ".." & RefEnd)
                End If

                l.Accession = Own_Al.Sam.QName
                Return l
            Else
                Dim l As Bio.IO.GenBank.ILocation
                If IsReverse = True Then
                    l = lBuilder.GetLocation("complement(" & RefStart & ".." & RefEnd & ")")
                Else
                    l = lBuilder.GetLocation(RefStart & ".." & RefEnd)
                End If
                l.Accession = Own_Al.Sam.QName
                Dim Exons As New List(Of ILocation)
                Exons.Add(lBuilder.GetLocation(RefStart & ".." & Introns(0).Ref_Start - 1))
                For i1 = 0 To Introns.Count - 2
                    Exons.Add(lBuilder.GetLocation(Introns(i1).Ref_End + 1 & ".." & Introns(i1 + 1).Ref_Start - 1))
                Next
                Exons.Add(lBuilder.GetLocation(Introns.Last.Ref_End + 1 & ".." & RefEnd))
                Exons = (From x In Exons Where x.LocationEnd - x.LocationStart > 0).ToList
                If IsReverse = True Then
                    l.SubLocations.First.Operator = LocationOperator.Join
                    l.SubLocations.First.SubLocations.AddRange(Exons)
                Else
                    l.Operator = LocationOperator.Join
                    l.SubLocations.AddRange(Exons)
                End If

                Return l
            End If
        End Function

        <Extension()>
        Public Function To_BioLocation(SAM As Bio.IO.SAM.SAMAlignedSequence) As Bio.IO.GenBank.Location
            Dim Own_Al As New Szunyi.Sam.Alignment.Own_Al(SAM)
            Dim Introns = From x In Own_Al.Parts Where x.Type = Own_Al.Type.Intron
            Dim RefStart As Integer = 0
            If Own_Al.Parts.First.Type = Own_Al.Type.Soft_Clip Then
                RefStart = Own_Al.Parts(1).Ref_Start
            Else
                RefStart = Own_Al.Parts.First.Ref_Start
            End If
            Dim RefEnd As Integer = 0
            If Own_Al.Parts.Last.Type = Own_Al.Type.Soft_Clip Then
                RefEnd = Own_Al.Parts(Own_Al.Parts.Count - 2).Ref_End
            Else
                RefEnd = Own_Al.Parts.Last.Ref_End
            End If

            Dim IsReverse As Boolean = False
            If Own_Al.Sam.Flag = Bio.IO.SAM.SAMFlags.QueryOnReverseStrand Then
                IsReverse = True
            End If
            Dim lBuilder As New Bio.IO.GenBank.LocationBuilder
            If Introns.Count = 0 Then
                Dim l As ILocation
                If IsReverse = True Then
                    l = lBuilder.GetLocation("complement(" & RefStart & ".." & RefEnd & ")")
                Else
                    l = lBuilder.GetLocation(RefStart & ".." & RefEnd)
                End If

                l.Accession = Own_Al.Sam.QName
                Return l
            Else
                Dim l As Bio.IO.GenBank.ILocation
                If IsReverse = True Then
                    l = lBuilder.GetLocation("complement(" & RefStart & ".." & RefEnd & ")")
                Else
                    l = lBuilder.GetLocation(RefStart & ".." & RefEnd)
                End If
                l.Accession = Own_Al.Sam.QName
                Dim Exons As New List(Of ILocation)
                Exons.Add(lBuilder.GetLocation(RefStart & ".." & Introns(0).Ref_Start - 1))
                For i1 = 0 To Introns.Count - 2
                    Exons.Add(lBuilder.GetLocation(Introns(i1).Ref_End + 1 & ".." & Introns(i1 + 1).Ref_Start - 1))
                Next
                Exons.Add(lBuilder.GetLocation(Introns.Last.Ref_End + 1 & ".." & RefEnd))
                Exons = (From x In Exons Where x.LocationEnd - x.LocationStart > 0).ToList
                If IsReverse = True Then
                    l.SubLocations.First.Operator = LocationOperator.Join
                    l.SubLocations.First.SubLocations.AddRange(Exons)
                Else
                    l.Operator = LocationOperator.Join
                    l.SubLocations.AddRange(Exons)
                End If

                Return l
            End If
        End Function


    End Module
End Namespace