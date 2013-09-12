Imports System.Security.Cryptography

Public Class frmMain


    Private Sub btnLottery_Click(sender As Object, e As EventArgs) Handles btnLottery.Click
        lbResults.Items.Clear()
        Dim names As New List(Of String)
        names = GetNames()
        Dim hat As New List(Of String)
        Dim iCount As Integer = names.Count
        For Each item As String In names
            For x As Integer = 1 To iCount
                hat.Add(item)
            Next
            iCount = iCount - 1
        Next
        While hat.Count > 0
            lbResults.Items.Add(Draw(hat))
        End While
    End Sub

    Private Function Draw(names As List(Of String)) As String
        Dim sName As String = ""
        Try
            If names.Count > 1 Then
                names.Sort(New Randomizer(Of String)())
                GenerateRandomNumbers(names.Count)
                Dim index As Integer = CInt(pickRandomNumberInRange(0, names.Count - 1))
                If index < names.Count - 1 Then
                    sName = names(index)
                End If
            Else
                sName = names(0)
            End If

            While names.Contains(sName)
                names.Remove(sName)
            End While
        Catch ex As Exception
            sName = Draw(names)
        End Try

        Return sName

    End Function

    Private RandomNumbers As New List(Of Decimal)

    Private Sub GenerateRandomNumbers(ByVal cnt As Integer)
        While RandomNumbers.Count < cnt
            Dim num As Decimal = GetRandomNumber()
            If Not RandomNumbers.Contains(num) Then RandomNumbers.Add(num)
        End While
    End Sub

    Private Function pickRandomNumberInRange(min As Integer, max As Integer) As Integer
        Dim rand As Integer = PickRandomNumber()
        If rand >= min And rand <= max Then
            Return rand
        ElseIf rand < min Then
            Return min
        ElseIf rand > max Then
            Return rand Mod max
        End If
        Return rand
    End Function

    Private Function PickRandomNumber() As Decimal
        If RandomNumbers.Count < 100 Then GenerateRandomNumbers(1000)
        Dim index As Integer = CInt(GetRandomNumber(RandomNumbers.Count))
        While index > RandomNumbers.Count - 1
            index = GetRandomNumber(RandomNumbers.Count)
        End While
        Dim retVal As Decimal = RandomNumbers(index)
        RandomNumbers.RemoveAt(index)
        Return retVal
    End Function

    Private Function GetRandomNumber(Optional MaxValue As Decimal = 1000000) As Decimal
        If MaxValue < 1 Then MaxValue = 1
        Randomize()
        Dim rnd = New Random()
        Dim nextValue As Decimal = rnd.Next(1001) * DateTime.Now.Millisecond
        Return nextValue Mod MaxValue
    End Function

    Private Function GetNames() As List(Of String)
        Dim names As New List(Of String)
        For Each item As String In lbHat.Items
            If names.Contains(item) Then
                MessageBox.Show("Can't use the same name twice.")
                Return New List(Of String)
            Else
                names.Add(item)
            End If
        Next
        Return names
    End Function

    Private Sub btnRandom_Click(sender As Object, e As EventArgs) Handles btnRandom.Click
        lbResults.Items.Clear()
        Dim names As New List(Of String)
        names = GetNames()
        While names.Count > 0
            lbResults.Items.Add(Draw(names))
        End While
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        If lbHat.SelectedIndex > -1 Then
            lbHat.Items.RemoveAt(lbHat.SelectedIndex)
            If lbHat.Items.Count > 0 Then
                lbHat.SelectedIndex = lbHat.Items.Count - 1
            End If
            SetHatCount()
        End If
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If txtName.Text.Length > 0 Then
            Dim sItems As String() = txtName.Text.Split(",")
            For Each s As String In sItems
                lbHat.Items.Add(Trim(s))
            Next
            lbHat.SelectedIndex = lbHat.Items.Count - 1
            txtName.Text = ""
            txtName.Focus()
            SetHatCount()
        End If
    End Sub

    Public Class Randomizer(Of T)
        Implements IComparer(Of T)

        ''// Ensures different instances are sorted in different orders
        Private Shared Salter As New Random() ''// only as random as your seed
        Private Salt As Integer
        Public Sub New()
            Salt = Salter.Next(Integer.MinValue, Integer.MaxValue)
        End Sub

        Private Shared sha As New SHA1CryptoServiceProvider()
        Private Function HashNSalt(ByVal x As Integer) As Integer
            Dim b() As Byte = sha.ComputeHash(BitConverter.GetBytes(x))
            Dim r As Integer = 0
            For i As Integer = 0 To b.Length - 1 Step 4
                r = r Xor BitConverter.ToInt32(b, i)
            Next

            Return r Xor Salt
        End Function

        Public Function Compare(x As T, y As T) As Integer _
            Implements IComparer(Of T).Compare

            Return HashNSalt(x.GetHashCode()).CompareTo(HashNSalt(y.GetHashCode()))
        End Function
    End Class

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Delete Then
            btnRemove.PerformClick()
        End If
    End Sub

    Private Sub SetHatCount()
        If lbHat.Items.Count = 0 Then
            Label1.Text = "Names in hat:"
        Else
            Label1.Text = "Names in hat (" & lbHat.Items.Count & "):"
        End If
    End Sub
End Class
