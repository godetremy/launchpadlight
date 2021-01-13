Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.IO.Compression


Public Class Home
    Private Snds As New MultiSounds

    Private Declare Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Integer, ByVal hwndCallback As Integer) As Integer

    Dim Page As Byte = 1
    Dim SoundPack As String = "Moving On"
    Dim PadNumber As Byte
    Dim Boucle As Boolean
    Dim MidiIn As String
    Dim PlayingSong As Integer
    Dim PlayLoop As Boolean = False
    Dim PadPlayingLoop As Byte

    Public Declare Function midiInGetNumDevs Lib "winmm.dll" () As Integer
    Public Declare Function midiInGetDevCaps Lib "winmm.dll" Alias "midiInGetDevCapsA" (ByVal uDeviceID As Integer, ByRef lpCaps As MIDIINCAPS, ByVal uSize As Integer) As Integer
    Public Declare Function midiInOpen Lib "winmm.dll" (ByRef hMidiIn As Integer, ByVal uDeviceID As Integer, ByVal dwCallback As MidiInCallback, ByVal dwInstance As Integer, ByVal dwFlags As Integer) As Integer
    Public Declare Function midiInStart Lib "winmm.dll" (ByVal hMidiIn As Integer) As Integer
    Public Declare Function midiInStop Lib "winmm.dll" (ByVal hMidiIn As Integer) As Integer
    Public Declare Function midiInReset Lib "winmm.dll" (ByVal hMidiIn As Integer) As Integer
    Public Declare Function midiInClose Lib "winmm.dll" (ByVal hMidiIn As Integer) As Integer

    Public Delegate Function MidiInCallback(ByVal hMidiIn As Integer, ByVal wMsg As UInteger, ByVal dwInstance As Integer, ByVal dwParam1 As Integer, ByVal dwParam2 As Integer) As Integer
    Public ptrCallback As New MidiInCallback(AddressOf MidiInProc)
    Public Const CALLBACK_FUNCTION As Integer = &H30000
    Public Const MIDI_IO_STATUS = &H20

    Public Delegate Sub DisplayDataDelegate(ByVal dwParam1)

    Public Structure MIDIINCAPS
        Dim wMid As Int16 ' Manufacturer ID
        Dim wPid As Int16 ' Product ID
        Dim vDriverVersion As Integer ' Driver version
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Dim szPname As String ' Product Name
        Dim dwSupport As Integer ' Reserved
    End Structure

    Dim hMidiIn As Integer
    Dim StatusByte As Byte
    Dim DataByte1 As Byte
    Dim DataByte2 As Byte
    Dim MonitorActive As Boolean = False
    Dim HideMidiSysMessages As Boolean = False

    Function MidiInProc(ByVal hMidiIn As Integer, ByVal wMsg As UInteger, ByVal dwInstance As Integer, ByVal dwParam1 As Integer, ByVal dwParam2 As Integer) As Integer
        If MonitorActive = True Then
            TextBox1.Invoke(New DisplayDataDelegate(AddressOf DisplayData), New Object() {dwParam1})
        End If
    End Function

    Private Sub DisplayData(ByVal dwParam1)
        If ((HideMidiSysMessages = True) And ((dwParam1 And &HF0) = &HF0)) Then
            Exit Sub
        Else
            StatusByte = (dwParam1 And &HFF)
            DataByte1 = (dwParam1 And &HFF00) >> 8
            DataByte2 = (dwParam1 And &HFF0000) >> 16
            TextBox1.AppendText(String.Format("{0:X2} {1:X2} {2:X2}{3}", StatusByte, DataByte1, DataByte2, vbCrLf))
        End If
    End Sub
    Sub pad()
            If My.Computer.FileSystem.FileExists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".wav") Then
                If Boucle = False Then
                    Snds.AddSound("pad" & PlayingSong, My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".wav")
                    Snds.Play("pad" & PlayingSong)
                    PlayingSong = PlayingSong + 1
                Else
                    If PlayLoop = False Or Not PadPlayingLoop = PadNumber Then
                        My.Computer.Audio.Play(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".wav", AudioPlayMode.BackgroundLoop)
                        PlayLoop = True
                        PadPlayingLoop = PadNumber
                    Else
                        My.Computer.Audio.Stop()
                        PlayLoop = False
                    End If
                End If
            Else
                MsgBox("Impossible de trouver le fichier audio !" & vbNewLine & "Merci de vérifier son emplacement.", MsgBoxStyle.Critical, "Erreur lors de la lecture du fichier audio")
            End If
    End Sub

    Sub padcolor()

        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Config\PadColor\pad" & PadNumber & ".txt") Then

        Else
            MsgBox("Le dossier selectionnez ne contient pas de fichier artiste !" & vbNewLine & "Merci de vérifiez son emplacement !", vbCritical, "Impossible de trouver le fichier artiste.txt")
            Label3.Text = "(null)"
        End If
    End Sub
    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectorPack.SelectedIndexChanged
        SoundPack = SelectorPack.Text

        If My.Computer.FileSystem.FileExists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Config\title.txt") Then
            Label2.Text = My.Computer.FileSystem.ReadAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Config\title.txt")
        Else
            MsgBox("Le dossier selectionnez ne contient pas de fichier titre !" & vbNewLine & "Merci de vérifiez son emplacement !", vbCritical, "Impossible de trouver le fichier title.txt")
            Label2.Text = "(null)"
        End If

        If My.Computer.FileSystem.FileExists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Config\artist.txt") Then
            Label3.Text = My.Computer.FileSystem.ReadAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Config\" & "artist.txt")
        Else
            MsgBox("Le dossier selectionnez ne contient pas de fichier artiste !" & vbNewLine & "Merci de vérifiez son emplacement !", vbCritical, "Impossible de trouver le fichier artiste.txt")
            Label3.Text = "(null)"
        End If



    End Sub

    Sub pad0_MouseDown() Handles pad0.MouseDown
        PadNumber = 0
        If My.Computer.Keyboard.ShiftKeyDown = False Then
            If pad0.ForeColor = Color.Cyan Then
                Boucle = True
            Else
                Boucle = False
            End If
            pad()
        Else

        End If
    End Sub

    Sub pad1_MouseDown() Handles pad1.MouseDown
        PadNumber = 1
        If pad1.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad2_MouseDown() Handles pad2.MouseDown
        PadNumber = 2
        If pad2.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad3_MouseDown() Handles pad3.MouseDown
        PadNumber = 3
        If pad3.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad4_MouseDown() Handles pad4.MouseDown
        PadNumber = 4
        If pad4.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad5_MouseDown() Handles pad5.MouseDown
        PadNumber = 5
        If pad5.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad6_MouseDown() Handles pad6.MouseDown
        PadNumber = 6
        If pad6.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad7_MouseDown() Handles pad7.MouseDown
        PadNumber = 7
        If pad7.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad8_MouseDown() Handles pad8.MouseDown
        PadNumber = 8
        If pad8.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad9_MouseDown() Handles pad9.MouseDown
        PadNumber = 9
        If pad9.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad10_MouseDown() Handles pad10.MouseDown
        PadNumber = 10
        If pad10.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad11_MouseDown() Handles pad11.MouseDown
        PadNumber = 11
        If pad11.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad12_MouseDown() Handles pad12.MouseDown
        PadNumber = 12
        If pad12.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad13_MouseDown() Handles pad13.MouseDown
        PadNumber = 13
        If pad13.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad14_MouseDown() Handles pad14.MouseDown
        PadNumber = 14
        If pad14.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad15_MouseDown() Handles pad15.MouseDown
        PadNumber = 15
        If pad15.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad16_MouseDown() Handles pad16.MouseDown
        PadNumber = 16
        If pad16.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad17_MouseDown() Handles pad17.MouseDown
        PadNumber = 17
        If pad17.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad18_MouseDown() Handles pad18.MouseDown
        PadNumber = 18
        If pad18.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad19_MouseDown() Handles pad19.MouseDown
        PadNumber = 19
        If pad19.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad20_MouseDown() Handles pad20.MouseDown
        PadNumber = 20
        If pad20.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad21_MouseDown() Handles pad21.MouseDown
        PadNumber = 21
        If pad21.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad22_MouseDown() Handles pad22.MouseDown
        PadNumber = 22
        If pad22.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad23_MouseDown() Handles pad23.MouseDown
        PadNumber = 23
        If pad23.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad24_MouseDown() Handles pad24.MouseDown
        PadNumber = 24
        If pad24.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad25_MouseDown() Handles pad25.MouseDown
        PadNumber = 25
        If pad25.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad26_MouseDown() Handles pad26.MouseDown
        PadNumber = 26
        If pad26.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad27_MouseDown() Handles pad27.MouseDown
        PadNumber = 27
        If pad27.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad28_MouseDown() Handles pad28.MouseDown
        PadNumber = 28
        If pad28.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad29_MouseDown() Handles pad29.MouseDown
        PadNumber = 29
        If pad29.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad30_MouseDown() Handles pad30.MouseDown
        PadNumber = 30
        If pad30.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad31_MouseDown() Handles pad31.MouseDown
        PadNumber = 31
        If pad31.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad32_MouseDown() Handles pad32.MouseDown
        PadNumber = 32
        If pad32.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad33_MouseDown() Handles pad33.MouseDown
        PadNumber = 33
        If pad33.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad34_MouseDown() Handles pad34.MouseDown
        PadNumber = 34
        If pad34.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad35_MouseDown() Handles pad35.MouseDown
        PadNumber = 35
        If pad35.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad36_MouseDown() Handles pad36.MouseDown
        PadNumber = 36
        If pad36.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad37_MouseDown() Handles pad37.MouseDown
        PadNumber = 37
        If pad37.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad38_MouseDown() Handles pad38.MouseDown
        PadNumber = 38
        If pad38.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad39_MouseDown() Handles pad39.MouseDown
        PadNumber = 39
        If pad39.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad40_MouseDown() Handles pad40.MouseDown
        PadNumber = 40
        If pad40.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad41_MouseDown() Handles pad41.MouseDown
        PadNumber = 41
        If pad41.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad42_MouseDown() Handles pad42.MouseDown
        PadNumber = 42
        If pad42.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad43_MouseDown() Handles pad43.MouseDown
        PadNumber = 43
        If pad43.ForeColor = Color.Cyan Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad44_MouseDown() Handles pad44.MouseDown
        PadNumber = 44
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad45_MouseDown() Handles pad45.MouseDown
        PadNumber = 45
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad46_MouseDown() Handles pad46.MouseDown
        PadNumber = 46
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad47_MouseDown() Handles pad47.MouseDown
        PadNumber = 47
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad48_MouseDown() Handles pad48.MouseDown
        PadNumber = 48
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad49_MouseDown() Handles pad49.MouseDown
        PadNumber = 49
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad50_MouseDown() Handles pad50.MouseDown
        PadNumber = 50
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad51_MouseDown() Handles pad51.MouseDown
        PadNumber = 51
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad52_MouseDown() Handles pad52.MouseDown
        PadNumber = 52
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad53_MouseDown() Handles pad53.MouseDown
        PadNumber = 53
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad54_MouseDown() Handles pad54.MouseDown
        PadNumber = 54
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad55_MouseDown() Handles pad55.MouseDown
        PadNumber = 55
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad56_MouseDown() Handles pad56.MouseDown
        PadNumber = 56
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad57_MouseDown() Handles pad57.MouseDown
        PadNumber = 57
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad58_MouseDown() Handles pad58.MouseDown
        PadNumber = 58
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad59_MouseDown() Handles pad59.MouseDown
        PadNumber = 59
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad60_MouseDown() Handles pad60.MouseDown
        PadNumber = 60
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad61_MouseDown() Handles pad61.MouseDown
        PadNumber = 61
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad62_MouseDown() Handles pad62.MouseDown
        PadNumber = 62
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Sub pad63_MouseDown() Handles pad63.MouseDown
        PadNumber = 63
        If My.Computer.FileSystem.FileExists("C:\Users\Dj Rémix\Documents\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad" & PadNumber & ".loop") Then
            Boucle = True
        Else
            Boucle = False
        End If
        pad()
    End Sub

    Private Sub Advance_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Page1.Checked = True Then
            Page = 1
        End If
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Page2.Checked = True Then
            Page = 2
        End If
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Page3.Checked = True Then
            Page = 3
        End If
    End Sub

    Private Sub RadioButton4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Page4.Checked = True Then
            Page = 4
        End If
    End Sub

    Private Sub RadioButton5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Page5.Checked = True Then
            Page = 5
        End If
    End Sub

    Private Sub RadioButton6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Page6.Checked = True Then
            Page = 6
        End If
    End Sub

    Private Sub RadioButton8_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Page7.Checked = True Then
            Page = 7
        End If
    End Sub

    Private Sub RadioButton7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Page8.Checked = True Then
            Page = 8
        End If
    End Sub

    Private Sub Home_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim Dossiers As Array = IO.Directory.GetDirectories(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\")
        For Each Valeur In Dossiers
            SelectorPack.Items.Add(Valeur.split("\")(Valeur.split("\").length - 1))
        Next
        Me.Show()
        If midiInGetNumDevs() = 0 Then 
        End If

        Dim InCaps As New MIDIINCAPS
        Dim DevCnt As Integer

        For DevCnt = 0 To (midiInGetNumDevs - 1)
            midiInGetDevCaps(DevCnt, InCaps, Len(InCaps))
            ComboBox1.Items.Add(InCaps.szPname)
        Next DevCnt

        'If My.Computer.FileSystem.FileExists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad0.wav") Then
        'Snds.AddSound("0pad01", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\Moving On\Sounds\Page1\pad0.wav")
        ' Else
        'pad0.FlatAppearance.BorderColor = Color.Gray
        'End If
        ' ProgressBar1.Value = 1

        ' If My.Computer.FileSystem.FileExists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad1.wav") Then
        ' Snds.AddSound("0pad11", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\Moving On\Sounds\Page1\pad1.wav")
        ' Else
        ' pad1.FlatAppearance.BorderColor = Color.Gray
        '  End If
        ' ProgressBar1.Value = 2
        '
        ' If My.Computer.FileSystem.FileExists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\" & SoundPack & "\Sounds\Page" & Page & "\pad2.wav") Then
        'Snds.AddSound("0pad21", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\Moving On\Sounds\Page1\pad2.wav")
        '  Else
        ' pad2.FlatAppearance.BorderColor = Color.Gray
        ' End If
        ' ProgressBar1.Value = 3
        ' Panel1.Visible = False
        Me.Visible = False
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim DeviceID As Integer = ComboBox1.SelectedIndex
        midiInOpen(hMidiIn, DeviceID, ptrCallback, 0, CALLBACK_FUNCTION Or MIDI_IO_STATUS)
        midiInStart(hMidiIn)
        MonitorActive = True
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged

        If TextBox1.Text.Contains("90 51 7F") Then
            pad0.BackColor = Color.White
            pad0_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 51 00") Then
            pad0.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 52 7F") Then
            pad1.BackColor = Color.White
            pad1_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 52 00") Then
            pad1.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 53 7F") Then
            pad2.BackColor = Color.White
            pad2_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 53 00") Then
            pad2.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 54 7F") Then
            pad3.BackColor = Color.White
            pad3_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 54 00") Then
            pad3.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 55 7F") Then
            pad4.BackColor = Color.White
            pad4_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 55 00") Then
            pad4.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 56 7F") Then
            pad5.BackColor = Color.White
            pad5_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 56 00") Then
            pad5.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 57 7F") Then
            pad6.BackColor = Color.White
            pad6_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 57 00") Then
            pad6.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 58 7F") Then
            pad7.BackColor = Color.White
            pad7_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 58 00") Then
            pad7.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 47 7F") Then
            pad8.BackColor = Color.White
            pad8_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 47 00") Then
            pad8.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 48 7F") Then
            pad9.BackColor = Color.White
            pad9_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 48 00") Then
            pad9.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 49 7F") Then
            pad10.BackColor = Color.White
            pad10_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 49 00") Then
            pad10.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 4A 7F") Then
            pad11.BackColor = Color.White
            pad11_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 4A 00") Then
            pad11.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 4B 7F") Then
            pad12.BackColor = Color.White
            pad12_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 4B 00") Then
            pad12.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 4C 7F") Then
            pad13.BackColor = Color.White
            pad13_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 4C 00") Then
            pad13.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 4D 7F") Then
            pad14.BackColor = Color.White
            pad14_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 4D 00") Then
            pad14.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 4E 7F") Then
            pad15.BackColor = Color.White
            pad15_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 4E 00") Then
            pad15.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 3D 7F") Then
            pad16.BackColor = Color.White
            pad16_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 3D 00") Then
            pad16.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 3E 7F") Then
            pad17.BackColor = Color.White
            pad17_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 3E 00") Then
            pad17.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 3F 7F") Then
            pad18.BackColor = Color.White
            pad18_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 3F 00") Then
            pad18.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 40 7F") Then
            pad19.BackColor = Color.White
            pad19_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 40 00") Then
            pad19.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 41 7F") Then
            pad20.BackColor = Color.White
            pad20_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 41 00") Then
            pad20.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 42 7F") Then
            pad21.BackColor = Color.White
            pad21_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 42 00") Then
            pad21.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 43 7F") Then
            pad22.BackColor = Color.White
            pad22_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 43 00") Then
            pad22.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 44 7F") Then
            pad23.BackColor = Color.White
            pad23_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 44 00") Then
            pad23.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 33 7F") Then
            pad24.BackColor = Color.White
            pad24_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 33 00") Then
            pad24.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 34 7F") Then
            pad25.BackColor = Color.White
            pad25_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 34 00") Then
            pad25.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 35 7F") Then
            pad26.BackColor = Color.White
            pad26_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 35 00") Then
            pad26.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 36 7F") Then
            pad27.BackColor = Color.White
            pad27_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 36 00") Then
            pad27.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 37 7F") Then
            pad28.BackColor = Color.White
            pad28_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 37 00") Then
            pad28.BackColor = Color.Black
            TextBox1.Clear()
        End If


        If TextBox1.Text.Contains("90 38 7F") Then
            pad29.BackColor = Color.White
            pad29_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 38 00") Then
            pad29.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 39 7F") Then
            pad30.BackColor = Color.White
            pad30_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 39 00") Then
            pad30.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 3A 7F") Then
            pad31.BackColor = Color.White
            pad31_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 3A 00") Then
            pad31.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 29 7F") Then
            pad32.BackColor = Color.White
            pad32_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 29 00") Then
            pad32.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 2A 7F") Then
            pad33.BackColor = Color.White
            pad33_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 2A 00") Then
            pad33.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 2B 7F") Then
            pad34.BackColor = Color.White
            pad34_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 2B 00") Then
            pad34.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 2C 7F") Then
            pad35.BackColor = Color.White
            pad35_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 2C 00") Then
            pad35.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 2D 7F") Then
            pad36.BackColor = Color.White
            pad36_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 2D 00") Then
            pad36.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 2E 7F") Then
            pad37.BackColor = Color.White
            pad37_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 2E 00") Then
            pad37.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 2F 7F") Then
            pad38.BackColor = Color.White
            pad38_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 2F 00") Then
            pad38.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 30 7F") Then
            pad39.BackColor = Color.White
            pad39_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 30 00") Then
            pad39.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 1F 7F") Then
            pad40.BackColor = Color.White
            pad40_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 1F 00") Then
            pad40.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 20 7F") Then
            pad41.BackColor = Color.White
            pad41_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 20 00") Then
            pad41.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 21 7F") Then
            pad42.BackColor = Color.White
            pad42_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 21 00") Then
            pad42.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 22 7F") Then
            pad43.BackColor = Color.White
            pad43_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 22 00") Then
            pad43.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 23 7F") Then
            pad44.BackColor = Color.White
            pad44_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 23 00") Then
            pad44.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 24 7F") Then
            pad45.BackColor = Color.White
            pad45_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 24 00") Then
            pad45.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 25 7F") Then
            pad46.BackColor = Color.White
            pad46_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 25 00") Then
            pad46.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 26 7F") Then
            pad47.BackColor = Color.White
            pad47_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 26 00") Then
            pad47.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 15 7F") Then
            pad48.BackColor = Color.White
            pad48_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 15 00") Then
            pad48.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 16 7F") Then
            pad49.BackColor = Color.White
            pad49_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 16 00") Then
            pad49.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 17 7F") Then
            pad50.BackColor = Color.White
            pad50_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 17 00") Then
            pad50.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 18 7F") Then
            pad51.BackColor = Color.White
            pad51_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 18 00") Then
            pad51.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 19 7F") Then
            pad52.BackColor = Color.White
            pad52_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 19 00") Then
            pad52.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 1A 7F") Then
            pad53.BackColor = Color.White
            pad53_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 1A 00") Then
            pad53.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 1B 7F") Then
            pad54.BackColor = Color.White
            pad54_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 1B 00") Then
            pad54.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 1C 7F") Then
            pad55.BackColor = Color.White
            pad55_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 1C 00") Then
            pad55.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 0B 7F") Then
            pad56.BackColor = Color.White
            pad56_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 0B 00") Then
            pad56.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 0C 7F") Then
            pad57.BackColor = Color.White
            pad57_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 0C 00") Then
            pad57.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 0D 7F") Then
            pad58.BackColor = Color.White
            pad58_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 0D 00") Then
            pad58.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 0E 7F") Then
            pad59.BackColor = Color.White
            pad59_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 0E 00") Then
            pad59.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 0F 7F") Then
            pad60.BackColor = Color.White
            pad60_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 0F 00") Then
            pad60.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 10 7F") Then
            pad61.BackColor = Color.White
            pad61_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 10 00") Then
            pad61.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 11 7F") Then
            pad62.BackColor = Color.White
            pad62_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 11 00") Then
            pad62.BackColor = Color.Black
            TextBox1.Clear()
        End If

        If TextBox1.Text.Contains("90 12 7F") Then
            pad63.BackColor = Color.White
            pad63_MouseDown()
            TextBox1.Clear()
        End If
        If TextBox1.Text.Contains("90 12 00") Then
            pad63.BackColor = Color.Black
            TextBox1.Clear()
        End If
    End Sub

    Private Sub Label4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label4.Click

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ComboBox1.Items.Clear()
        If midiInGetNumDevs() = 0 Then
            ComboBox1.Text = "Aucun appareil MIDI connécté"
        End If

        Dim InCaps As New MIDIINCAPS
        Dim DevCnt As Integer

        For DevCnt = 0 To (midiInGetNumDevs - 1)
            midiInGetDevCaps(DevCnt, InCaps, Len(InCaps))
            ComboBox1.Items.Add(InCaps.szPname)
        Next DevCnt
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        SelectorPack.Items.Clear()
        Dim Dossiers As Array = IO.Directory.GetDirectories(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\")
        For Each Valeur In Dossiers
            SelectorPack.Items.Add(Valeur.split("\")(Valeur.split("\").length - 1))
        Next
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        ZipFile.ExtractToDirectory(OpenFileDialog1.FileName, My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\")
        SelectorPack.Items.Clear()
        Dim Dossiers As Array = IO.Directory.GetDirectories(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\LaunchpadLight\SoundPack\")
        For Each Valeur In Dossiers
            SelectorPack.Items.Add(Valeur.split("\")(Valeur.split("\").length - 1))
        Next
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        If SettingPanel.Visible = False Then
            SettingPanel.Visible = True
            SoundpackPanel.Visible = False
        Else
            SettingPanel.Visible = False
        End If
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        'If SoundpackPanel.Visible = False Then
        'SoundpackPanel.Visible = True
        '  SettingPanel.Visible = False
        '  Else
        '  SoundpackPanel.Visible = False
        '   End If
        SelectPack.Show()
        Me.Close()
    End Sub

    Private Sub pad0_MouseDown(sender As Object, e As MouseEventArgs) Handles pad0.MouseDown

    End Sub
    Private Sub pad1_MouseDown(sender As Object, e As MouseEventArgs) Handles pad1.MouseDown

    End Sub
    Private Sub pad2_MouseDown(sender As Object, e As MouseEventArgs) Handles pad2.MouseDown

    End Sub
    Private Sub pad3_MouseDown(sender As Object, e As MouseEventArgs) Handles pad3.MouseDown

    End Sub
    Private Sub pad4_MouseDown(sender As Object, e As MouseEventArgs) Handles pad4.MouseDown

    End Sub
    Private Sub pad5_MouseDown(sender As Object, e As MouseEventArgs) Handles pad5.MouseDown

    End Sub
    Private Sub pad6_MouseDown(sender As Object, e As MouseEventArgs) Handles pad6.MouseDown

    End Sub
    Private Sub pad7_MouseDown(sender As Object, e As MouseEventArgs) Handles pad7.MouseDown

    End Sub
    Private Sub pad8_MouseDown(sender As Object, e As MouseEventArgs) Handles pad8.MouseDown

    End Sub
    Private Sub pad9_MouseDown(sender As Object, e As MouseEventArgs) Handles pad9.MouseDown

    End Sub
    Private Sub pad10_MouseDown(sender As Object, e As MouseEventArgs) Handles pad10.MouseDown

    End Sub
    Private Sub pad11_MouseDown(sender As Object, e As MouseEventArgs) Handles pad11.MouseDown

    End Sub
    Private Sub pad12_MouseDown(sender As Object, e As MouseEventArgs) Handles pad12.MouseDown

    End Sub
    Private Sub pad13_MouseDown(sender As Object, e As MouseEventArgs) Handles pad13.MouseDown

    End Sub
    Private Sub pad14_MouseDown(sender As Object, e As MouseEventArgs) Handles pad14.MouseDown

    End Sub
    Private Sub pad15_MouseDown(sender As Object, e As MouseEventArgs) Handles pad15.MouseDown

    End Sub
    Private Sub pad16_MouseDown(sender As Object, e As MouseEventArgs) Handles pad16.MouseDown

    End Sub
    Private Sub pad17_MouseDown(sender As Object, e As MouseEventArgs) Handles pad17.MouseDown

    End Sub
    Private Sub pad18_MouseDown(sender As Object, e As MouseEventArgs) Handles pad18.MouseDown

    End Sub
    Private Sub pad19_MouseDown(sender As Object, e As MouseEventArgs) Handles pad19.MouseDown

    End Sub
    Private Sub pad20_MouseDown(sender As Object, e As MouseEventArgs) Handles pad20.MouseDown

    End Sub
    Private Sub pad21_MouseDown(sender As Object, e As MouseEventArgs) Handles pad21.MouseDown

    End Sub
    Private Sub pad22_MouseDown(sender As Object, e As MouseEventArgs) Handles pad22.MouseDown

    End Sub
    Private Sub pad23_MouseDown(sender As Object, e As MouseEventArgs) Handles pad23.MouseDown

    End Sub
    Private Sub pad24_MouseDown(sender As Object, e As MouseEventArgs) Handles pad24.MouseDown

    End Sub
    Private Sub pad25_MouseDown(sender As Object, e As MouseEventArgs) Handles pad25.MouseDown

    End Sub
    Private Sub pad26_MouseDown(sender As Object, e As MouseEventArgs) Handles pad26.MouseDown

    End Sub
    Private Sub pad27_MouseDown(sender As Object, e As MouseEventArgs) Handles pad27.MouseDown

    End Sub
    Private Sub pad28_MouseDown(sender As Object, e As MouseEventArgs) Handles pad28.MouseDown

    End Sub
    Private Sub pad29_MouseDown(sender As Object, e As MouseEventArgs) Handles pad29.MouseDown

    End Sub
    Private Sub pad30_MouseDown(sender As Object, e As MouseEventArgs) Handles pad30.MouseDown

    End Sub
    Private Sub pad31_MouseDown(sender As Object, e As MouseEventArgs) Handles pad31.MouseDown

    End Sub
    Private Sub pad32_MouseDown(sender As Object, e As MouseEventArgs) Handles pad32.MouseDown

    End Sub
    Private Sub pad33_MouseDown(sender As Object, e As MouseEventArgs) Handles pad33.MouseDown

    End Sub
    Private Sub pad34_MouseDown(sender As Object, e As MouseEventArgs) Handles pad34.MouseDown

    End Sub
    Private Sub pad35_MouseDown(sender As Object, e As MouseEventArgs) Handles pad35.MouseDown

    End Sub
    Private Sub pad36_MouseDown(sender As Object, e As MouseEventArgs) Handles pad36.MouseDown

    End Sub
    Private Sub pad37_MouseDown(sender As Object, e As MouseEventArgs) Handles pad37.MouseDown

    End Sub
    Private Sub pad38_MouseDown(sender As Object, e As MouseEventArgs) Handles pad38.MouseDown

    End Sub
    Private Sub pad39_MouseDown(sender As Object, e As MouseEventArgs) Handles pad39.MouseDown

    End Sub
    Private Sub pad40_MouseDown(sender As Object, e As MouseEventArgs) Handles pad40.MouseDown

    End Sub
    Private Sub pad41_MouseDown(sender As Object, e As MouseEventArgs) Handles pad41.MouseDown

    End Sub
    Private Sub pad42_MouseDown(sender As Object, e As MouseEventArgs) Handles pad42.MouseDown

    End Sub
    Private Sub pad43_MouseDown(sender As Object, e As MouseEventArgs) Handles pad43.MouseDown

    End Sub
    Private Sub pad44_MouseDown(sender As Object, e As MouseEventArgs) Handles pad44.MouseDown

    End Sub
    Private Sub pad45_MouseDown(sender As Object, e As MouseEventArgs) Handles pad45.MouseDown

    End Sub
    Private Sub pad46_MouseDown(sender As Object, e As MouseEventArgs) Handles pad46.MouseDown

    End Sub
    Private Sub pad47_MouseDown(sender As Object, e As MouseEventArgs) Handles pad47.MouseDown

    End Sub
    Private Sub pad48_MouseDown(sender As Object, e As MouseEventArgs) Handles pad48.MouseDown

    End Sub
    Private Sub pad49_MouseDown(sender As Object, e As MouseEventArgs) Handles pad49.MouseDown

    End Sub
    Private Sub pad50_MouseDown(sender As Object, e As MouseEventArgs) Handles pad50.MouseDown

    End Sub
    Private Sub pad51_MouseDown(sender As Object, e As MouseEventArgs) Handles pad51.MouseDown

    End Sub
    Private Sub pad52_MouseDown(sender As Object, e As MouseEventArgs) Handles pad52.MouseDown

    End Sub
    Private Sub pad53_MouseDown(sender As Object, e As MouseEventArgs) Handles pad53.MouseDown

    End Sub
    Private Sub pad54_MouseDown(sender As Object, e As MouseEventArgs) Handles pad54.MouseDown

    End Sub
    Private Sub pad55_MouseDown(sender As Object, e As MouseEventArgs) Handles pad55.MouseDown

    End Sub
    Private Sub pad56_MouseDown(sender As Object, e As MouseEventArgs) Handles pad56.MouseDown

    End Sub
    Private Sub pad57_MouseDown(sender As Object, e As MouseEventArgs) Handles pad57.MouseDown

    End Sub
    Private Sub pad58_MouseDown(sender As Object, e As MouseEventArgs) Handles pad58.MouseDown

    End Sub
    Private Sub pad59_MouseDown(sender As Object, e As MouseEventArgs) Handles pad59.MouseDown

    End Sub
    Private Sub pad60_MouseDown(sender As Object, e As MouseEventArgs) Handles pad60.MouseDown

    End Sub
    Private Sub pad61_MouseDown(sender As Object, e As MouseEventArgs) Handles pad61.MouseDown

    End Sub
    Private Sub pad62_MouseDown(sender As Object, e As MouseEventArgs) Handles pad62.MouseDown

    End Sub
    Private Sub pad63_MouseDown(sender As Object, e As MouseEventArgs) Handles pad63.MouseDown

    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        Me.Close()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Me.Show()
    End Sub

    Private Sub Guna2Button7_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Guna2RatingStar1_ValueChanged(sender As Object, e As EventArgs) Handles Guna2RatingStar1.ValueChanged
        Guna2Button5.Enabled = True
    End Sub

    Private Sub Guna2Button5_Click(sender As Object, e As EventArgs) Handles Guna2Button5.Click
        MsgBox("Merci de votre avis !")
        SettingPanel.Visible = False
    End Sub

    Private Sub Page1_CheckedChanged(sender As Object, e As EventArgs) Handles Page1.CheckedChanged
        Page = 1
    End Sub

    Private Sub Page2_CheckedChanged(sender As Object, e As EventArgs) Handles Page2.CheckedChanged
        Page = 2
    End Sub

    Private Sub Page3_CheckedChanged(sender As Object, e As EventArgs) Handles Page3.CheckedChanged
        Page = 3
    End Sub

    Private Sub Page4_CheckedChanged(sender As Object, e As EventArgs) Handles Page4.CheckedChanged
        Page = 4
    End Sub

    Private Sub Page5_CheckedChanged(sender As Object, e As EventArgs) Handles Page5.CheckedChanged
        Page = 5
    End Sub

    Private Sub Page6_CheckedChanged(sender As Object, e As EventArgs) Handles Page6.CheckedChanged
        Page = 6
    End Sub

    Private Sub Page7_CheckedChanged(sender As Object, e As EventArgs) Handles Page7.CheckedChanged
        Page = 7
    End Sub

    Private Sub Page8_CheckedChanged(sender As Object, e As EventArgs) Handles Page8.CheckedChanged
        Page = 8
    End Sub

End Class
