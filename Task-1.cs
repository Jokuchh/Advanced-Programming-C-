//Ex1
class Kompaine
{
    public string nom;
    public List<Appart> apparts;
    public List<Transaction> ListAchat; // On sépare les transactions en ventes et achats pour permettre de calculer le bénéfice
    public List<Transaction> ListVente;
    public Kompaine(string _nom, List<Appart> A, List<Transaction> achats, List<Transaction> ventes)
    {
        nom = _nom;
        apparts = A;
        ListAchat = achats;     
        ListVente = ventes;Kompaine
    }

    public double Superficie(Batiment B)
    {
        double S = 0;
        List<Appart> appartList = new List<Appart>();
        for (int i = 0; i < apparts.Count; i++)
        {
            if (B.Apparts.Contains(appartList[i]))
            {
                appartList.Add(apparts[i]);
            }
        }
        foreach(Appart A in appartList)
        {
            S += A.superficie;
        }
        return S;
    }

    public double Superficie(Quartier Q)
    {
        double S = 0;
        List<Appart> appartList = new List<Appart>();
        for (int i = 0; i < apparts.Count; i++)
        {
            for (int j = 0; j < Q.Batiments.Count; j++)
            if (Q.Batiments[j].Apparts.Contains(apparts[i]))
            {
                appartList.Add(apparts[i]);
            }
        }
        foreach (Appart A in appartList)
        {
            S += A.superficie;
        }
        return S;
    }

    public double BeneficeIndividu()
    {
        double b = 0.0;
        foreach (Transaction t in ListAchat)
        {
            if (t.Client.GetTypeC() == "I") //Chaque client a son type, tous les individus sont "typés" I, toutes les sociétés sont typées S
            {
                b -= t.prix;                //Pour chaque transaction de ListAchat on enlève le prix de la transaction au bénéfice, et inversement pour ListVente 
            }
        }
        foreach (Transaction t in ListVente)
        { 
            if (t.Client.GetTypeC()=="I")
            {
                b+=t.prix;
            }
        }
        return b;
    }

    public double BeneficeSociété()
    {
        double b = 0.0;
        foreach (Transaction t in ListAchat)
        {
            if (t.Client.GetTypeC() == "S")
            {
                b -= t.prix;
            }
        }
        foreach (Transaction t in ListVente)
        {
            if (t.Client.GetTypeC() == "S")
            {
                b += t.prix;
            }
        }
        return b;
    }

}

class Appart
{
    public int NoLot;
    int NbrePieces;
    public double superficie;
    public Appart(int NLot, int Npieces, double sup)
    {
        NoLot = NLot;
        NbrePieces = Npieces;
        superficie = sup;
    }
}

class Batiment
{
    string nom;
    public List<Appart> Apparts;
    string adresse;
    int NbreEtages;
    public Batiment(string Nom,List<Appart> A, string adr, int nbEtages)
    {
        nom = Nom;
        Apparts = A;
        adresse = adr;
        NbreEtages = nbEtages;
    }
}

class Quartier
{
    string nom;
    public List<Batiment> Batiments;
    public Quartier(string Nom, List<Batiment> B)
    {
        nom = Nom;
        Batiments = B;
    }
}

interface Client
{
    double GetMoney();
    void SetMoney(double m);
    List<Appart> GetBiens();
    string GetNom();
    string GetTypeC();
}
class Individu : Client
{
    public string nom;
    List<Appart> Biens;
    public double argent;
    public string TypeClient = "I";
    public Individu(string _nom, int Argent, List<Appart> biens)
    {
        nom= _nom;
        argent = Argent;
        Biens = biens;
    }
    public double GetMoney()
    {
        return argent;
    }
    public void SetMoney(double m)
    {
        argent = m;
    }
    public List<Appart> GetBiens()
    {
        return Biens;
    }
    public string GetNom()
    {
        return nom;
    }
    public string GetTypeC()
    {
        return TypeClient;
    }
}

class Société : Client
{
    public string nom;
    List<Appart> Biens;
    public double argent;
    public string TypeClient = "S";
    public Société(string _nom, int Argent, List<Appart> biens)
    {
        nom= _nom;
        argent = Argent;
        Biens = biens;
    }
    public double GetMoney()
    {
        return argent;
    }
    public void SetMoney(double m)
    {
        argent = m;
    }

    public List<Appart> GetBiens()
    {
        return Biens;
    }
    public string GetNom()
    {
        return nom;
    }

    public string GetTypeC()
    {
        return TypeClient;
    }
}
class Transaction //Chaque transaction s'effectue entre une compagnie et un client, elle concerne une liste d'appartements et a un prix
{
    public Compagnie compagnie;
    public Client Client;
    List<Appart> Apparts;
    public double prix;
    string date;
    public Transaction(Compagnie C, Client _client, List<Appart> L, double p, string Date)
    {
        compagnie = C;
        Client = _client;
        Apparts = L;
        prix = p;
        date = Date;
    }

    public void AchatCompagnie() // La compagnie achète la liste d'appartements au client
    {
        int flag = 1;
        // On crée une variable qui va nous permettre de sortir de la boucle si jamais on rencontre un problème qui peut faire que la transaction ne peut pas aboutir,
        // ou alors que la transaction est aboutie. flag=1 : transaction en cours; flag=-1 : transaction impossible; flag=0 : transaction finie
        while (flag == 1)
        {
            if (Apparts.Count == 0) flag = -1;
            // Si la liste d'appartements est vide, alors la transaction n'est pas effectuée
            else
            {
                int compteur = 0;
                // On crée un compteur qui va nous permettre de savoir si le client détient bien tous les appartements qu'il vend
                foreach (Appart A in Apparts)
                {
                    if (Client.GetBiens().Contains(A)) compteur++;
                    if (compagnie.apparts.Contains(A)) flag = 0; // Si la compagnie détient déjà un appartement qu'on lui vend, alors la transaction n'est pas effectuée
                }
                if (compteur != Apparts.Count) flag = -1;
                else
                {
                    Client.SetMoney(Client.GetMoney() + prix); // On met à jour l'argent du client
                    try
                    {
                        foreach (Appart A in Apparts)
                        {
                            Client.GetBiens().Remove(A);
                            compagnie.apparts.Add(A);
                            compagnie.ListAchat.Add(this); // On met la transaction qui vient d'être effectuée dans la liste des achats de la compagnie
                            Console.WriteLine("Le " + date + ", la compagnie " + compagnie.nom + " a acheté l'appartement " + A.NoLot + " à " + Client.GetNom() + " pour " + prix + " euros.");
                        }
                        flag = 0;
                    }
                    catch { flag = 0; }
                }  
            }
        }
        if (flag == -1) Console.WriteLine("L'achat n'a pas pu se concrétiser.");
        else Console.WriteLine("L'achat a pu être effectué");
    }

    public void VenteCompagnie() // La compagnie vend la liste d'appartements au client
    {
        int flag = 1;   
        // Même principe que le flag précédent
        while (flag == 1)
        {
            if ((Client.GetMoney() < prix) || (Apparts.Count == 0)) flag = -1;
            // Si le client n'a pas suffisamment d'argent ou que la liste d'appartements est vide, alors la transaction n'est pas effectuée
            else
            {
                int compteur = 0;
                // On crée un compteur qui va nous permettre de savoir si la compagnie détient bien tous les appartements qu'elle vend
                foreach (Appart A in Apparts)
                {
                    if (compagnie.apparts.Contains(A)) compteur++;
                    if (Client.GetBiens().Contains(A)) flag = -1; // Si le client détient déjà un appartement qu'on lui vend, alors la transaction n'est pas effectuée
                }
                if (compteur != Apparts.Count) flag = -1;
                else
                {
                    Client.SetMoney(Client.GetMoney() - prix); // On met à jour l'argent du client
                    try
                    {
                        foreach (Appart A in Apparts)
                        {
                            Client.GetBiens().Add(A);
                            compagnie.apparts.Remove(A);
                            compagnie.ListVente.Add(this); // On met la transaction qui vient d'être effectuée dans la liste des ventes de la compagnie
                            Console.WriteLine("Le " + date + ", la compagnie " + compagnie.nom + " a vendu l'appartement " + A.NoLot + " à " + Client.GetNom() + " pour " + prix + " euros.");
                        }
                        flag = 0;
                    }
                    catch { flag = 0; }
                }

            }
        }
        if (flag == -1) Console.WriteLine("La vente n'a pas pu se concrétiser.");
        else Console.WriteLine("La vente a pu être effectuée");
    }
}


class DM
{
    public static void Main(string[] args)
    {
        Appart Ap1 = new Appart(203, 2, 25);
        Appart Ap2 = new Appart(209, 2, 30);
        Appart Ap3 = new Appart(207, 2, 20);
        Appart Ap4 = new Appart(051, 3, 50);
        
        List<Appart> L1 = new List<Appart>();
        List<Appart> L2 = new List<Appart>();
        List<Appart> LSociété = new List<Appart>();
        List<Appart> ListeAchat = new List<Appart>();
        List<Appart> ListeVente = new List<Appart>();

        L1.Add(Ap1);
        L2.Add(Ap2);
        ListeAchat.Add(Ap1);
        ListeVente.Add(Ap4);
        LSociété.Add(Ap3);
        LSociété.Add(Ap4);

        Batiment Bat1 = new Batiment("Batiment 1", L1, "adresse1", 6);
        
        List<Batiment> LBat1 = new List<Batiment>();
        
        Quartier Q1 = new Quartier("Nations", LBat1);
        
        Client C1 = new Individu("Client 1",500000,L1);
        Client C2 = new Société("Client 2",50,L2);
        
        List<Transaction> Ventes = new List<Transaction>();
        List<Transaction> Achats = new List<Transaction>();
        
        Compagnie Comp = new Compagnie("Compagnie 1", LSociété, Achats, Ventes);
        
        Transaction T1 = new Transaction(Comp, C1, ListeAchat, 789, "01/01/2022");
        T1.AchatCompagnie();
        Transaction T2 = new Transaction(Comp, C2, ListeVente, 78789, "12/12/2012");
        T2.VenteCompagnie();
        C2.SetMoney(C2.GetMoney() + 80000);
        T2.VenteCompagnie();
        Console.WriteLine("La compagnie " + Comp.nom + " a effectué un bénéfice de " + Comp.BeneficeIndividu() + " euros avec des individus et un bénéfice de " + Comp.BeneficeSociété() + " euros avec des sociétés");
    }
}