using AppControleFinanceiro.Views;

namespace AppControleFinanceiro;

public partial class App : Application
{
	public App(TransactionList listpage) // Obtida através da injecao de dependencia
	{
		InitializeComponent();

        // Define a página de inicializaçao do projeto
        MainPage = new NavigationPage (listpage); 
	}
}	
