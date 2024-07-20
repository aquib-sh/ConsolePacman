using System.Text;


public class Obstacle
{
    public required int X { get; set; }
    public required int Y { get; set; }  
    public required string Value { get; set; }
}

public class ObstacleGenerator
{
    private Random random = new Random();   

    private int AVAILABLE_HEIGHT, AVAILABLE_WIDTH;
    public ObstacleGenerator(int height, int width)
    {
        AVAILABLE_HEIGHT = height;  
        AVAILABLE_WIDTH = width;        
    }

    public Obstacle[] GenerateObstacles(ushort nObstacles = 1)
    {
        Obstacle[] obstacles = new Obstacle[nObstacles];

        for (int i = 0; i < obstacles.Length; i++) 
        { 
            obstacles[i] = new Obstacle { X = random.Next(0, AVAILABLE_WIDTH), Y = random.Next(0, AVAILABLE_HEIGHT), Value = "⛰︎" };
        }
        return obstacles;       
    }

    public void PlaceObstacles(Obstacle[] obstacles)
    {
        foreach (Obstacle obstacle in obstacles)
        {
            Console.SetCursorPosition(obstacle.X, obstacle.Y); 
            Console.Write(obstacle.Value);      
        }
    }
}

public class CollisionDetector
{
    private Obstacle[] obstacles;
    private int sensitivity;

    public CollisionDetector(Obstacle[] obstacles)
    {
        this.obstacles = obstacles;
        sensitivity = 2;
    }

    public void setSensitivity(int sensitivity)
    {
        this.sensitivity = sensitivity;     
    }
    
    private bool doesFallInZone(int localPosition, int alienObjectPosition)
    {
        return alienObjectPosition.Equals(localPosition)     // equal to position
            || alienObjectPosition > localPosition && alienObjectPosition < (localPosition + sensitivity)   // greater than local position but falls inside zone 
            || alienObjectPosition < localPosition && alienObjectPosition > (localPosition - sensitivity);  // greater than local position but falls inside zone
    }

    public bool DoesCollide(int pos_x, int pos_y)
    { 
        return obstacles.Any(obstacle => doesFallInZone(obstacle.X, pos_x) && doesFallInZone(obstacle.Y, pos_y)); 
    }
}


public class Player
{
    private readonly string CHARACTER = "🐭";
    private int MOVEMENT_SPEED = 5;

    private int POS_X = 0;
    private int POS_Y = 0;

    private int PREVIOUS_POS_X = 0;
    private int PREVIOUS_POS_Y = 0;

    private CollisionDetector _collisionDetector;    

    public Player(CollisionDetector collisionDetector, string avatar = "🐭")
    {
        CHARACTER = avatar;     
        _collisionDetector = collisionDetector; 
    }

    public void setMovementSpeed(int speed) 
    { 
        this.MOVEMENT_SPEED = speed;
    }

    public void MoveLeft()
    {
        int newPos = POS_X - MOVEMENT_SPEED;

        if (newPos >= 0 && !_collisionDetector.DoesCollide(newPos, POS_Y))
        {
            POS_X = newPos;
            Console.CursorLeft = POS_X;
        }
    }

    public void MoveRight()
    {
        int newPos = POS_X + MOVEMENT_SPEED;       

        if (newPos < Console.BufferWidth && !_collisionDetector.DoesCollide(newPos, POS_Y))
        {
            POS_X = newPos;       
            Console.CursorLeft = POS_X;
        }
    }

    public void MoveUp()
    {
        int newPos = POS_Y - MOVEMENT_SPEED;  
        if ( newPos >= 0 && !_collisionDetector.DoesCollide(POS_X, newPos))
        {
            POS_Y = newPos;
            Console.CursorTop = POS_Y;       
        }
    }

    public void MoveDown()
    {
        int newPos = POS_Y + MOVEMENT_SPEED;       

        if (newPos < Console.BufferHeight && !_collisionDetector.DoesCollide(POS_X, newPos))
        {
            POS_Y = newPos;       
            Console.CursorTop = POS_Y;       
        }
    }

    private void ErasePreviousPosition()
    {
        Console.SetCursorPosition(PREVIOUS_POS_X, PREVIOUS_POS_Y);
        Console.Write(" ");
        Console.SetCursorPosition(POS_X, POS_Y);    
    }

    private void RecordCurrentPosition()
    {
        PREVIOUS_POS_X = POS_X;
        PREVIOUS_POS_Y = POS_Y;
    }

    public void Display()
    {
        ErasePreviousPosition();    
        Console.Write(CHARACTER);  
        RecordCurrentPosition();    

    }

    public void RefreshPosition()
    {
        Console.SetCursorPosition(POS_X, POS_Y);     
    }
}

 
public class Pacman
{
    ObstacleGenerator obstacleGenerator = new ObstacleGenerator(Console.BufferHeight, Console.BufferWidth);

    CollisionDetector collisionDetector;
    Player player;

    public Pacman()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Obstacle[] obstacles = obstacleGenerator.GenerateObstacles(20);
        obstacleGenerator.PlaceObstacles(obstacles);        
        collisionDetector = new CollisionDetector(obstacles);

        collisionDetector.setSensitivity(2);

        player = new Player(collisionDetector);
        player.setMovementSpeed(3);

        player.RefreshPosition();   
        player.Display();   

    }
    
    public void UserInputEventLoop()
    {
        ConsoleKeyInfo input = Console.ReadKey(intercept:true);

        bool rePrint = true;

        switch(input.KeyChar)
        {
            case 'w':
                player.MoveUp();
                break;

            case 's':
                player.MoveDown();
                break;

            case 'd':   
                player.MoveRight();        
                break;  

            case 'a':   
                player.MoveLeft(); 
                break;  
                
            default:
                rePrint = false;        
                break;
        }

        if (rePrint)
        {
            player.RefreshPosition();       
            player.Display();
        }
    }

    public void Run()
    {
        player.Display();

        while (true)
        {
            UserInputEventLoop();   
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {

       new Pacman().Run();     
   
    }
}