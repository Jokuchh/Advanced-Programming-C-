Console.WriteLine("Modélisation chute balle avec vitesse initiale de 1500 m/s");
Parabole balle_rapide = new Parabole(1500);
balle_rapide.Modelisation(5, 20);
Console.WriteLine("Avance balle rapide de 0 à 5s");
balle_rapide.Avance(5);
balle_rapide.Afficher_valeur_actuelle();
Console.WriteLine("Modélisation chute balle avec vitesse initiale de 10 m/s");
Parabole balle_lente = new Parabole(10);
balle_lente.Modelisation(0.2, 20);
Console.WriteLine("Avance balle lente de 0 à 5s");
balle_lente.Avance(1);
balle_lente.Afficher_valeur_actuelle();
Console.WriteLine("Dérivée de balle rapide à t = 15s");
Derive derive_balle_rapide = new Derive(balle_rapide, 0.1);
derive_balle_rapide.Avance(10);
derive_balle_rapide.Afficher_valeur_actuelle();
Console.WriteLine("Dérivée de balle lente à t = 1.2s");
Derive derive_balle_lente = new Derive(balle_lente, 0.1);
derive_balle_lente.Avance(0.2);
derive_balle_lente.Afficher_valeur_actuelle();
Console.WriteLine("Accélération de balle lente (dérivée d'une dérivée)");
Derive acceleration_balle_lente = new Derive(derive_balle_lente, 0.1);
acceleration_balle_lente.Afficher_valeur_actuelle();
Console.WriteLine("Accélération de balle rapide (dérivée d'une dérivée)");
Derive acceleration_balle_rapide = new Derive(derive_balle_rapide, 5);
acceleration_balle_rapide.Afficher_valeur_actuelle();


interface IAnimable
{
    double Valeur { get; }
    void Avance(double dt);
}

class Parabole : IAnimable
{
    // Modélise uniquement la chute d'une balle
    private double _hauteur; 
    private double _vitesse_init;
    private double _acceleration; 
    private double _temps_actuel;
    private double _hauteur_max;
    private double _temps_h_max;

    public Parabole(double vitesse) //Cas de chute libre depuis le sol, seule la vitesse initiale est à indiquer
        //(c'est une chute libre avec lancé donc pas de propulsion sur la balle, impossible de choisir l'accélération)
        //Pas de prise en compte d'un angle, on rentre en paramètre juste la vitesse en Z
    {
        _hauteur = 0; // Balle lancée depuis le sol -> 0 mais placé pour être changé si nécessaire
        _vitesse_init = vitesse; // Au choix selon la puissance du lancé en m/s mais doit être positive (on n'envoies pas de balle au travers du sol)
        _acceleration = -9.81; // Sur terre -> -g puisque z est orienté vers le haut donc -9,81 m/s²
        _temps_actuel = 0; //Temps depuis le départ de la balle en secondes, permet de retrouver d'anciennes positions
        _temps_h_max = (-_vitesse_init / _acceleration);
        _hauteur_max = _temps_h_max * _temps_h_max * (_acceleration / 2) + _vitesse_init * _temps_h_max;
    }
    public double Valeur { get { return _hauteur; } }
    public double temps_actuel { get { return _temps_actuel; } }
    public void Avance(double dt)
    {
        _temps_actuel = _temps_actuel + dt;
        _hauteur = _temps_actuel*_temps_actuel*(_acceleration / 2)  + _vitesse_init * _temps_actuel; //Formule z = -g/2 * t² + v(0) * t + h(0) (h(0) = 0 donc pas présent)
        if(_hauteur < 0) 
        {
            //Cette condition représente la fin de chute, !!pas de rebonds!! donc si la hauteur de la balle atteit
            //une valeur négative, alors la chute est terminée (donc hauteur = 0)
            _hauteur = 0;
        }

    }

    public void Modelisation(double dt,double nombre_de_points) //Modélise la chute de la balle avec un nombre de point et un écart de temps défini
    {
        double _temps_précédent = _temps_actuel; // pour garder en mémoire l'avance déjà réalisé si on veut continuer l'avance pas à pas
        _temps_actuel = 0;
        for(int i = 0; i < nombre_de_points; i++)
        {
            Console.WriteLine("pour t = " + _temps_actuel + " secondes, on a h = " + _hauteur," mètre");
            Avance(dt);
        }
        _temps_actuel = _temps_précédent;
    }

    public void Afficher_valeur_actuelle()
    {
        Console.WriteLine("la balle est à une hauteur de " + _hauteur);
    }
    public void Reset_avance() //Permet de faire repartir de zero la fonction "Avance"
    {
        _temps_actuel = 0;
    }
}

class Derive : IAnimable
{
    private double _valeur; //Valeur de la dérivée à t = (temps du IAnimable étudié + temps totale de l'avance de la dérivée)
    private double _precision; //écart entre les deux points utilisés dans le calcul de la dérivée
    private double _difference_temps_derivee_par_rapport_ianimable; //Temps totale de l'avance de la dérivée
    private IAnimable _p;//Objet IAnimable à étudier
    
    public Derive(IAnimable p, double precision) //Permet de réaliser une dérivée à un point donnée avec la précision souhaitée et l'avancer de dt en gardant la même précision
    {
        _p = p;
        _precision = Math.Abs(precision); //précision : ne peut pas être égal à 0 ou négative
        _difference_temps_derivee_par_rapport_ianimable = 0;
        double t1 = _precision / 2;
        double t2 = _precision * (-1);
        _p.Avance(t1);
        double p1 = _p.Valeur;
        _p.Avance(t2);
        double p2 = _p.Valeur;
        _p.Avance(t1); //Retourne l'objet IAnimable à sa valeur d'origine avant calcul de dérivée
        _valeur = (p1 - p2) / _precision;
    }

    public double Valeur { get { return _valeur; } }

    public void Avance(double dt)
    {
        _difference_temps_derivee_par_rapport_ianimable += dt;
        double t1 = _precision / 2 + _difference_temps_derivee_par_rapport_ianimable;
        double t2 = _precision * (-1);
        double tretour = t1 - 2 * _difference_temps_derivee_par_rapport_ianimable;
        _p.Avance(t1); 
        double p1 = _p.Valeur;
        _p.Avance(t2);
        double p2 = _p.Valeur;
        _p.Avance(tretour); //Retourne l'objet IAnimable à son avance d'origine avant calcul de dérivée, permettant de ne pas le modifier
        _valeur = (p1 - p2) / _precision;
    }

    public void Afficher_valeur_actuelle()
    {
        Console.WriteLine("la dérivée a pour valeur " + _valeur);
    }
    public void Reset_avance() //Permet de faire repartir de zero la fonction "Avance" et de la synchroniser avec l'objet IAnimable
    {
        _difference_temps_derivee_par_rapport_ianimable = 0;
        double t1 = _precision / 2;
        double t2 = _precision * (-1);
        _p.Avance(t1);
        double p1 = _p.Valeur;
        _p.Avance(t2);
        double p2 = _p.Valeur;
        _p.Avance(t1); //Retourne l'objet IAnimable à sa valeur d'origine avant calcul de dérivée, permettant de ne pas le modifier
        _valeur = (p1 - p2) / _precision;
    }
}
