using AppControleFinanceiro.Libraries.Utils;
using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using System.Text;

namespace AppControleFinanceiro.Views;

public partial class TransactionEdit : ContentPage
{
    private ITransactionRepository _repository; 
    private Transaction _transaction;

	public TransactionEdit(ITransactionRepository repository) // Injecao de dependencia no construtor
    {
		InitializeComponent();
        _repository = repository;
	}

    public void SetTransactionEdit(Transaction transaction)
    {
        _transaction = transaction;

        if (transaction.Type == TransactionType.Income)
            RadioIncome.IsChecked = true;
        else 
            RadioIncome.IsChecked = false;

        EntryName.Text = transaction.Name;
        DatePickerDate.Date = transaction.Date.Date;
        EntryValue.Text = transaction.Value.ToString();
    }

    private void TapGestureRecognizerTapped_ToClose(object sender, TappedEventArgs e)
    {
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

        KeyboardFixBugs.HideKeyboard();
        // Fechar a tela e avisa para atualizar
        Navigation.PopModalAsync();
        WeakReferenceMessenger.Default.Send<String>(string.Empty);

        // Mostra a quantidade de registros no banco via mensagem alert
        //var count = _repository.GetAll().Count;
        //App.Current.MainPage.DisplayAlert("Mensagem!", $"Existem {count} registro(s) no banco!", "OK");
    }

    //Salvar banco (Pegar os dados, Criar obj transaction, Enviar Repository)
    private void SaveTransactionInDatabase()
    {
        Transaction transaction = new Transaction() // Cria uma nova instancia
        {
            Id = _transaction.Id,
            Type = RadioIncome.IsChecked ? TransactionType.Income : TransactionType.Expenses,
            Name = EntryName.Text,
            Date = DatePickerDate.Date,
            Value = Math.Abs(double.Parse(EntryValue.Text))
        };

        // Pegando a injecao de dependencia
        _repository.Update(transaction);
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

        if (!string.IsNullOrEmpty(EntryValue.Text) && !double.TryParse(EntryValue.Text, out result))
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