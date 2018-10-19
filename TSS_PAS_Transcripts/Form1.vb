Imports Szunyi.BLs
Imports Szunyi.IO

Public Class Form1
    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Dim x As New FolderSelectDialog

        If x.ShowDialog = DialogResult.OK Then
            Dim Seq_File = Szunyi.IO.Pick_Up.Files(Szunyi.IO.File_Extensions.GenBank).ToList
            If IsNothing(Seq_File) = True Then Exit Sub
            Dim Seqs = Szunyi.IO.Import.Sequences.Parse(Seq_File)
            Dim cSeqs = Szunyi.Sequences.Common.Clone(Seqs)

            Dim Intron As New Settings.Intron
            Dim c As New Controls.Set_Console_Properties(Intron)
            If c.ShowDialog <> DialogResult.OK Then Exit Sub

            Dim TSS_PAS_TR As New Settings.TSS_PAS_TR
            Dim c2 As New Controls.Set_Console_Properties(TSS_PAS_TR)
            If c2.ShowDialog <> DialogResult.OK Then Exit Sub

            Dim CountBy As New Settings.Count_As
            Dim c3 As New Controls.Set_Console_Properties(CountBy)
            If c3.ShowDialog <> DialogResult.OK Then Exit Sub

            '   Dim res As New Dictionary(Of IO.FileInfo, List(Of Szunyi.Stat.Distribution_Result))
            Dim Dirs = Szunyi.IO.Directory.Get_Directories(x.FolderNames.ToList)

            Dim All_TSSs As New List(Of Basic_Location)
            Dim All_PASs As New List(Of Basic_Location)
            Dim All_PAs As New List(Of Basic_Location)
            cSeqs = Szunyi.Sequences.Common.Clone(Seqs)
            For Each D In Dirs
                Dim k As New TSS_PAS_Transcripts.Transcripts.Analysis(D, cSeqs.First, Intron, TSS_PAS_TR, CountBy)
                k.DoIt()
                All_PAs.AddRange(k.PAs)
                All_TSSs.AddRange(k.TSSs)
                All_PASs.AddRange(k.PASs)
            Next
            Dim ALL As New TSS_PAS_Transcripts.Transcripts.Analysis(Dirs.First.Parent, cSeqs.First, Intron, TSS_PAS_TR, CountBy)
            ALL.TSSs = All_TSSs
            ALL.PAs = All_PAs
            ALL.PASs = All_PASs
            ALL.DoIt()
        End If

    End Sub
End Class
