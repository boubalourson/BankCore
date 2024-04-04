using System;
using System.Collections.Generic;

namespace SystemeBancaireCentral
{
    public class CompteBancaire
    {
        private static int s_numeroCompteSemence = 1234567890;
        private List<Transaction> _toutesTransactions = new List<Transaction>();

        public string Numero { get; }
        public string Proprietaire { get; set; }
        public decimal Solde
        {
            get
            {
                decimal solde = 0;
                foreach (var transaction in _toutesTransactions)
                {
                    solde += transaction.Montant;
                }
                return solde;
            }
        }

        public CompteBancaire(string nom, decimal soldeInitial)
        {
            Numero = s_numeroCompteSemence.ToString();
            s_numeroCompteSemence++;
            Proprietaire = nom;
            EffectuerDepot(soldeInitial, DateTime.Now, "Solde initial");
        }

        public void EffectuerDepot(decimal montant, DateTime date, string note)
        {
            if (montant <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(montant), "Le montant du dépôt doit être positif");
            }
            var depot = new Transaction(montant, date, note);
            _toutesTransactions.Add(depot);
        }

        public void EffectuerRetrait(decimal montant, DateTime date, string note)
        {
            if (montant <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(montant), "Le montant du retrait doit être positif");
            }
            if (Solde - montant < 0)
            {
                throw new InvalidOperationException("Fonds insuffisants pour ce retrait");
            }
            var retrait = new Transaction(-montant, date, note);
            _toutesTransactions.Add(retrait);
        }

        public string ObtenirHistoriqueCompte()
        {
            var rapport = new System.Text.StringBuilder();

            decimal solde = 0;
            rapport.AppendLine("Date\t\tMontant\tSolde\tNote");
            foreach (var transaction in _toutesTransactions)
            {
                solde += transaction.Montant;
                rapport.AppendLine($"{transaction.Date.ToShortDateString()}\t{transaction.Montant}\t{solde}\t{transaction.Notes}");
            }

            return rapport.ToString();
        }
    }

    public class Transaction
    {
        public decimal Montant { get; }
        public DateTime Date { get; }
        public string Notes { get; }

        public Transaction(decimal montant, DateTime date, string note)
        {
            Montant = montant;
            Date = date;
            Notes = note;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bienvenue dans le Système Bancaire Central !");
            Console.Write("Entrez votre nom : ");
            string nom = Console.ReadLine();

            Console.Write("Entrez le solde initial : ");
            decimal soldeInitial;
            while (!decimal.TryParse(Console.ReadLine(), out soldeInitial) || soldeInitial < 0)
            {
                Console.WriteLine("Veuillez entrer un nombre positif valide pour le solde initial.");
                Console.Write("Entrez le solde initial : ");
            }

            var compte = new CompteBancaire(nom, soldeInitial);
            Console.WriteLine($"Le compte {compte.Numero} a été créé pour {compte.Proprietaire} avec un solde initial de {compte.Solde}.");

            bool continuerTransactions = true;
            while (continuerTransactions)
            {
                Console.WriteLine("\nChoisissez une action :");
                Console.WriteLine("1. Effectuer un dépôt");
                Console.WriteLine("2. Effectuer un retrait");
                Console.WriteLine("3. Afficher l'historique du compte");
                Console.WriteLine("4. Quitter");

                Console.Write("Entrez votre choix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        Console.Write("Entrez le montant du dépôt : ");
                        decimal montantDepot;
                        while (!decimal.TryParse(Console.ReadLine(), out montantDepot) || montantDepot < 0)
                        {
                            Console.WriteLine("Veuillez entrer un montant positif valide pour le dépôt.");
                            Console.Write("Entrez le montant du dépôt : ");
                        }
                        compte.EffectuerDepot(montantDepot, DateTime.Now, "Dépôt");
                        Console.WriteLine($"Dépôt de {montantDepot} effectué. Le solde actuel est de {compte.Solde}");
                        break;

                    case "2":
                        Console.Write("Entrez le montant du retrait : ");
                        decimal montantRetrait;
                        while (!decimal.TryParse(Console.ReadLine(), out montantRetrait) || montantRetrait < 0)
                        {
                            Console.WriteLine("Veuillez entrer un montant positif valide pour le retrait.");
                            Console.Write("Entrez le montant du retrait : ");
                        }
                        try
                        {
                            compte.EffectuerRetrait(montantRetrait, DateTime.Now, "Retrait");
                            Console.WriteLine($"Retrait de {montantRetrait} effectué. Le solde actuel est de {compte.Solde}");
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;

                    case "3":
                        Console.WriteLine(compte.ObtenirHistoriqueCompte());
                        break;

                    case "4":
                        continuerTransactions = false;
                        Console.WriteLine("Fermeture du programme. Merci !");
                        break;

                    default:
                        Console.WriteLine("Choix invalide. Veuillez entrer un nombre de 1 à 4.");
                        break;
                }
            }
        }
    }
}
