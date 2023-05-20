using System.Text;
using AppControleFinanceiro.Libraries.Utils;
using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;

namespace AppControleFinanceiro.Views;

public partial class TransactionAdd : ContentPage
{
    private ITransactionRepository _repository; // Injecao de dependencia
	public TransactionAdd(ITransactionRepository repository)
	{
		InitializeComponent();
        _repository = repository;
	}

    private void TapGestureRecognizerTapped_ToClose(object sender, TappedEventArgs e)
    {
        // Fix para fechar o teclado no android
        KeyboardFixBugs.HideKeyboard();
        Navigation.PopModalAsync();
    }

    private void OnButtonClicked_Save(object sender, EventArgs e)
    {
        // Valida os dados
        if (IsValidData() == false)
            return;

       // Salvar banco
        SaveTransactionInDatabase();

        // Fix para fechar o teclado no android
        KeyboardFixBugs.HideKeyboard();
        // Fechar a tela
        Navigation.PopModalAsync();

        WeakReferenceMessenger.Default.Send<String>(string.Empty);

        //// Exibe Mensagem na tela de quantos registros cadastrados
        //var count = _repository.GetAll().Count;
        //App.Current.MainPage.DisplayAlert("Mensagem!", $"Existem {count} registro(s) no banco!", "OK");

    }

    //Salvar banco (Pegar os dados, Criar obj transaction, Enviar Repository)
    private void SaveTransactionInDatabase()
    {
        Transaction transaction = new Transaction()
        {
            Type = RadioIncome.IsChecked ? TransactionType.Income : TransactionType.Expenses,
            Name = EntryName.Text,
            Date = DatePickerDate.Date,
            Value = Math.Abs(double.Parse(EntryValue.Text))
        };

        // Pegando a injecao de dependencia
        _repository.Add(transaction);
    }

    //Validar dados digitados
    private bool IsValidData()
    {
        bool valid = true;
        StringBuilder sb = new StringBuilder();

        // Valida o campo de nome da tarefa
        if (string.IsNullOrEmpty(EntryName.Text) || string.IsNullOrWhiteSpace(EntryName.Text))
        {
            sb.AppendLine("O campo 'Nome' deve ser preenchido!");
            valid = false;
        }
        // Valida o campo de valor da receita ou despesa
        if (string.IsNullOrEmpty(EntryValue.Text) || string.IsNullOrWhiteSpace(EntryValue.Text))
        {
            sb.AppendLine("O campo 'Valor' deve ser preenchido!");
            valid = false;
        }

        // Valida se é pode ser convertido para texto
        double result;

        if(!string.IsNullOrEmpty(EntryValue.Text) && !double.TryParse(EntryValue.Text, out result))
        {
            sb.AppendLine("O campo 'Valor' é inválido!");
            valid = false;
        }

        // Valida o campo de alertas para exibir
        if (valid == false)
        {
            LabelError.IsVisible = true;
            LabelError.Text = sb.ToString();
        }

        return valid;
    }
}