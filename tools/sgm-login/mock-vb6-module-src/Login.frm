VERSION 5.00
Begin VB.Form Login 
   Caption         =   "Login SGM-OC WEB"
   ClientHeight    =   2055
   ClientLeft      =   120
   ClientTop       =   450
   ClientWidth     =   7815
   KeyPreview      =   -1  'True
   LinkTopic       =   "Form1"
   ScaleHeight     =   2055
   ScaleWidth      =   7815
   StartUpPosition =   2  'CenterScreen
   Begin VB.TextBox oTxt_Sucursal 
      Height          =   375
      Left            =   825
      MaxLength       =   3
      TabIndex        =   1
      Text            =   "1"
      Top             =   750
      Width           =   810
   End
   Begin VB.CheckBox oChkUrl 
      Caption         =   "Abrir URL?"
      Height          =   255
      Left            =   810
      TabIndex        =   6
      Top             =   1650
      Width           =   1635
   End
   Begin VB.TextBox oTxt_Url 
      Height          =   375
      Left            =   825
      TabIndex        =   2
      Text            =   "http://localhost:3000/auth/?token="
      Top             =   1185
      Width           =   6975
   End
   Begin VB.CommandButton Command1 
      Caption         =   "(F3) Generar"
      Height          =   360
      Left            =   6270
      TabIndex        =   3
      Top             =   1665
      Width           =   1470
   End
   Begin VB.TextBox oTxtUsuario 
      Height          =   375
      Left            =   825
      MaxLength       =   15
      TabIndex        =   0
      Top             =   315
      Width           =   2160
   End
   Begin VB.Label Label2 
      AutoSize        =   -1  'True
      Caption         =   "Sucursal"
      Height          =   195
      Left            =   165
      TabIndex        =   7
      Top             =   825
      Width           =   615
   End
   Begin VB.Label Label4 
      AutoSize        =   -1  'True
      Caption         =   "Url"
      Height          =   195
      Left            =   165
      TabIndex        =   5
      Top             =   1260
      Width           =   195
   End
   Begin VB.Label Label1 
      AutoSize        =   -1  'True
      Caption         =   "Usuario"
      Height          =   195
      Left            =   165
      TabIndex        =   4
      Top             =   390
      Width           =   540
   End
End
Attribute VB_Name = "Login"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Declare Function ShellExecute Lib "shell32.dll" Alias "ShellExecuteA" (ByVal hwnd As Long, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As Long) As Long
Private Sub Command1_Click()

On Error GoTo ErrorSist:

If oTxtUsuario.Text = "" Then
    MsgBox "Ingrese usuario", vbInformation, "Atencion"
    Exit Sub
End If
If oTxt_Sucursal.Text = "" Then
    MsgBox "Ingrese una sucursal", vbInformation, "Atencion"
    Exit Sub
End If

Dim NetObj As EncryptionToVB6.EncryptedPasswordManager
Set NetObj = New EncryptionToVB6.EncryptedPasswordManager

oTxt_Url.Text = "http://localhost:3000/auth/?token=" & NetObj.Encryptation(oTxtUsuario.Text, Val(oTxt_Sucursal.Text))

If oChkUrl.Value = 1 Then
    Dim shell
    shell = ShellExecute(Me.hwnd, "Open", oTxt_Url.Text, &O0, &O0, SW_NORMAL)
End If

Exit Sub
ErrorSist:
    MsgBox Err.Description

End Sub

Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = vbKeyF3 Then
        Call Command1_Click
    End If
End Sub

