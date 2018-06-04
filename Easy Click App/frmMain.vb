Imports System.IO

Public Class frmMain

    Dim apppath As String
    Private Sub CreateConfigFile()

        Dim osStruct As String
        Dim appExe As String = txtMainApp.Text().Substring(txtSelectedFolder.Text().Length + 1)

        If rdb32bit.Checked Then
            osStruct = "x86"
        Else
            osStruct = "x64"
        End If
        Try
            Using writer As StreamWriter = New StreamWriter(txtSelectedFolder.Text + "\app.config")
                writer.WriteLine(txtAppName.Text + "^" + appExe + "^" + osStruct)
            End Using

            lblUpdate.Text = "Configuration File Created!"

        Catch ex As Exception

        End Try

    End Sub

    Private Sub SilentZip()
        Dim argcmd = "/k 7z.exe a -t7z " + """" + txtSelectedFolder.Text + "\myApp.7z" + """" + " " + """" + txtSelectedFolder.Text + "\\"""
        Dim startInfo As New ProcessStartInfo("cmd.exe")
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        startInfo.CreateNoWindow = True
        startInfo.UseShellExecute = False
        startInfo.Arguments = argcmd
        Process.Start(startInfo)


    End Sub

    Private Sub CreateExeFile()
        Dim argcmd = "/k  copy /b 7zS.sfx + config.txt + " + """" + txtSelectedFolder.Text + "\myApp.7z" + """" + " " + """" + txtSelectedFolder.Text + "\" + txtAppName.Text + "-Easy Install.exe" + """"
        Dim startInfo As New ProcessStartInfo("cmd.exe")
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        startInfo.CreateNoWindow = True
        startInfo.UseShellExecute = False
        startInfo.Arguments = argcmd
        Process.Start(startInfo)
    End Sub

    Public Sub BrowseDirectories_All()
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim path As String = FolderBrowserDialog1.SelectedPath
            txtSelectedFolder.Text = path
        End If
    End Sub

    Public Sub BrowseDirectories_MainExe()

        If apppath = "" Then
            Dim fd As OpenFileDialog = New OpenFileDialog()
            fd.Title = "Select Main Executable File"
            fd.InitialDirectory = txtSelectedFolder.Text
            fd.Filter = "Exe Files |*.exe"
            fd.FilterIndex = 2
            fd.RestoreDirectory = True
            If fd.ShowDialog() = DialogResult.OK Then
                txtMainApp.Text = fd.FileName
            End If
        End If

    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()

    End Sub

    Private Sub btnBrowseExe_Click(sender As Object, e As EventArgs) Handles btnBrowseExe.Click
        BrowseDirectories_MainExe()
    End Sub

    Private Sub btnBrowseFolder_Click(sender As Object, e As EventArgs) Handles btnBrowseFolder.Click
        BrowseDirectories_All()
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As EventArgs) Handles btnCreate.Click
        If txtSelectedFolder.Text = "" Or txtMainApp.Text = "" Or txtAppName.Text = "" Then
            MsgBox("Please supply the needed information before creating one click executable file!")
        Else
            lblUpdate.Visible = True
            lblUpdate.Text = "Please wait . . ."
            MsgBox(txtAppName.Text + " will be created!" + vbNewLine + "Click OK to continue...")

            CopyDeploymentTool()
            CreateConfigFile()
            Threading.Thread.Sleep(1000)
            SilentZip()
            Threading.Thread.Sleep(5000)
            CreateExeFile()

            lblUpdate.Visible = False
            DeleteTempConfig()
            Dim finish As DialogResult = MessageBox.Show("Executable File Created! Open destination Folder?",
                                                  "Completed!",
                                                  MessageBoxButtons.YesNo)
            If finish = DialogResult.Yes Then
                DeleteTempZip()
                Process.Start("explorer.exe", txtSelectedFolder.Text)
            Else
                DeleteTempZip()
            End If

            Clear()
        End If

    End Sub

    Private Sub txtSelectedFolder_TextChanged(sender As Object, e As EventArgs) Handles txtSelectedFolder.TextChanged
        If txtSelectedFolder.Text = "" Then
            btnBrowseExe.Enabled = False
        Else
            btnBrowseExe.Enabled = True
        End If
    End Sub

    Private Sub btnAbout_Click(sender As Object, e As EventArgs) Handles btnAbout.Click
        MsgBox("Easy Click Installer - Setup v1.0" +
               vbNewLine + vbNewLine + "Create your single executable installer in just simple steps!" +
               vbNewLine + "With the help of 7zip(open source), self extracting executable file is at hand." +
               vbNewLine + vbNewLine + "Check this out!")
    End Sub

    Private Sub Clear()
        btnBrowseExe.Enabled = False
        txtAppName.Clear()
        txtMainApp.Clear()
        txtSelectedFolder.Clear()

    End Sub

    Private Sub DeleteTempConfig()
        File.Delete(txtSelectedFolder.Text + "\app.config")
    End Sub

    Private Sub DeleteTempZip()

        If File.Exists(txtSelectedFolder.Text + "\myApp.7z") Then
            File.Delete(txtSelectedFolder.Text + "\myApp.7z")
        End If

        If File.Exists(txtSelectedFolder.Text + "\EasyInstaller.exe") Then
            File.Delete(txtSelectedFolder.Text + "\EasyInstaller.exe")
        End If

    End Sub

    Private Sub CopyDeploymentTool()


        Dim FileToCopy = Application.StartupPath.ToString + "\EasyInstaller.helper"
        Dim NewCopy = txtSelectedFolder.Text + "\EasyInstaller.exe"

        If Not File.Exists(NewCopy) Then
            If File.Exists(FileToCopy) = True Then
                File.Copy(FileToCopy, NewCopy)
            End If
        End If

    End Sub

    Dim move As Boolean
    Dim moveX, moveY As Integer

    Private Sub pnlHeader_MouseUp(sender As Object, e As MouseEventArgs) Handles pnlHeader.MouseUp
        move = False
    End Sub

    Private Sub pnlHeader_MouseDown(sender As Object, e As MouseEventArgs) Handles pnlHeader.MouseDown
        move = True
        moveX = Cursor.Position.X - Me.Left
        moveY = Cursor.Position.Y - Me.Top
    End Sub

    Private Sub pnlHeader_MouseMove(sender As Object, e As MouseEventArgs) Handles pnlHeader.MouseMove
        If move Then
            Me.Top = Cursor.Position.Y - moveY
            Me.Left = Cursor.Position.X - moveX
        End If
    End Sub

End Class
