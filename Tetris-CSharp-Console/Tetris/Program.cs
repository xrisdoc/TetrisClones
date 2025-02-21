// This Tetris clone is based on the original GAME BOY version of Tetris.
// We will try to keep it as true as possible where we can.
// So, for the screen size we will replicate the GAME BOY sreen grid of 20 columns and 18 rows.
// Within the screen, the game field, where the blocks are in play is played on a 10x18 grid, with the game pieces being 4x4 blocks.
// Areas outside of the game field are used for the score, level, and next block display.

Random randmoiser = new Random();
Console.CursorVisible = false;
bool isTestMode = true;

// There are 7 block types in Tetris, each with a unique shape.
// These blocks are actually refered to as Tetrominos.
// Each Tetromino is made up of 4 blocks.
// Each block is represented by a 4x4 grid of characters.
// The characters are either a space or an 'X' character.
// The 'X' character represents a block and the dot/period character represents an empty space.
// The 4x4 grid is represented as a string with 4 lines, each line containing 4 characters.
// The 7 Tetrominos are defined below:
string[] tetrominos = new string[7];

// I Block
tetrominos[0] = "..X." +
                "..X." +
                "..X." +
                "..X.";

// J BLOCK
tetrominos[1] = "..X." +
                "..X." +
                ".XX." +
                "....";
// L Block
tetrominos[2] = ".X.." +
               ".X.." +
               ".XX." +
               "....";

// O Block
tetrominos[3] = "...." +
               ".XX." +
               ".XX." +
               "....";
// S Block
tetrominos[4] = "...." +
                "..XX" +
                ".XX." +
                "....";
// T Block
tetrominos[5] = "...." +
                "XXX." +
                ".X.." +
                "....";
// Z Block
tetrominos[6] = "...." +
                ".XX." +
                "..XX" +
                "....";

string characterSet = " ABCDEFG=#.";

// The screen is a 20x18 grid of characters.
// These dimensions are used to define the size of the screen buffer.
int screenWidth = 20;
int screenHeight = 18;

// The screen buffer is a 1D array of integers.
// The screen is drawn to the console by converting the integers to characters.
// The integers are used as an index into a character set.
int[] screen = new int[screenWidth * screenHeight];

// The game playing field is a 10x18 grid of characters.
// The game field is where the blocks are in play.
int fieldWidth = 10;

// The game fields is a secondary screen buffer that will hold the accumulated blocks.
// The contents of the game field will eventually be transfered into the screen buffer at the relevant time.
int[] gameField = new int[screenWidth * screenHeight];

// The current block and the next block are randomly selected from the 7 Tetrominos.
// The blocks are represented as an integer value from 0 to 6.
// The integer value is an index into the Tetrominos array.
int currentBlock = randmoiser.Next(0, 6);
int nextBlock = randmoiser.Next(0, 6);

// Set the initial position and rotation of the current block.
// The current block will be drawn to the screen based on its position and rotation.
int currentRotation = 0;
int currentX = 3;
int currentY = 0;

// The score, block count, and line count are used to keep track of the player's progress.
// The score is calculated based on the number of blocks placed and lines cleared.
// The block count is the number of blocks placed by the player.
// The line count is the number of lines cleared by the player.
// The score, block count, and line count are displayed to the player at the end of the game.
int score = 0;
int blockCount = 0;
int lineCount = 0;

// Fill out the screen buffer
// The screen buffer is a 1D array of integers.
// Each integer represents a character on the screen.
// The integer value is an index into a character set.
// The character set is " ABCDEFG=#".
// The character set is used to draw the screen to the console.
// The screen buffer is filled with 0's to represent empty space.
// The screen buffer is then filled with 9's to represent the border of the game field.
// This initial fill done yo fefine the borders and specific areas of the screen.
for (int index = 0; index < screen.Length; index++)
{
    screen[index] = 0;

    int row = index / screenWidth % screenHeight;
    int col = index % screenWidth;

    if (col == 0 || col == 11 || col == 19)
    {
       screen[index] = 9;
    }
    else if ((row == 0 && col >= 11) || row == 17)
    {
        screen[index] = 9;
    }
}

// Set the initial variables for the game loop.
// The game loop is a while loop that runs until the game is over.
bool isGameOver = false;
bool forceDown = false;

// The game will start at a speed of 20.
// The speed of the game is controlled by the delay between frames.
int speed = 20;
int speedCount = 0;
List<int> clearedLines = [];

// The game loop updates the game state and draws the screen to the console.
// The game loop runs until the game is over.
while (!isGameOver)
{
    // Delay the game loop by to control the game speed.
    // The game speed is controlled by the delay between frames.
    // The delay is set to 25 milliseconds, which is equivalent to 40 frames per second.
    await Task.Delay(25);

    // Increment the speed count.
    // The speed count is used to control the speed of the game.
    speedCount++;

    // Check if the block should move down by 1 line.
    // The block will move down by 1 line if the speed count reaches the speed value.
    forceDown = speedCount == speed;

    // Clear the list of cleared lines.
    // The list of cleared lines is used to keep track of the lines that are full and need to be cleared.
    // Clearing this here will ensure that we only clear the lines that are full in the current frame.
    clearedLines.Clear();

    // Fill the relevant areas within the screen with the accumlated blocks.
    // We need to have this in place prior to any checks to see if the current block will fit.
    // Otherwise, we would have nothing to check against.
    SetGameFieldWithinScreenBuffer(screen, gameField, screenWidth, screenHeight);

    // Check if a key is pressed and update the game state accordingly.
    // We cannot use Console.ReadKey() at this point because it blocks the game loop.
    // We need to use Console.KeyAvailable to check if a key is pressed.
    // If a key is pressed, then we can read the key using Console.ReadKey(true).
    // The true parameter is used to prevent the key from being displayed on the console.
    // We will use the arrow keys to move the current block left, right, down, and the up arrow to rotate.
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true).Key;
        if (key == ConsoleKey.Escape)
        {
            // Quit the game if the escape key is pressed.
            // The game loop will exit and the game will be over.
            isGameOver = true;
        }
        else if (key == ConsoleKey.LeftArrow)
        {
            // Move the block left if the left arrow is pressed.
            // Only move the block left if it fits within the game field.
            // The block will move left by 1 column if it fits.
            if (CheckBlockFits(currentBlock, currentRotation, currentX - 1, currentY))
            {
                currentX -= 1;
            }
        }
        else if (key == ConsoleKey.RightArrow)
        {
            // Move the block right if the right arrow is pressed.
            // Only move the block right if it fits within the game field.
            // The block will move right by 1 column if it fits.
            if (CheckBlockFits(currentBlock, currentRotation, currentX + 1, currentY))
            {
                currentX += 1;
            }
        }
        else if (key == ConsoleKey.DownArrow)
        {
            // Move the block down if the down arrow is pressed.
            // Only move the block down if it fits within the game field.
            // The block will move down by 1 line if it fits.
            if (CheckBlockFits(currentBlock, currentRotation, currentX, currentY + 1))
            {
                currentY += 1;
            }
        }
        else if (isTestMode && key == ConsoleKey.UpArrow)
        {
            // Rotate the block if the up arrow is pressed.
            // But only game is currently in test mode.
            // Moving the block up is not a standard feature in Tetris.
            // This is for testing purposes only.
            if (CheckBlockFits(currentBlock, currentRotation, currentX, currentY + 1))
            {
                currentY -= 1;
            }
        }
        else
        {
            // Rotate the block if the spacebar is pressed.
            if (key == ConsoleKey.Spacebar)
            {
                // TODO:
                // Implement hold rotate to prevent multiple rotations in a single key press

                int nextRotation = currentRotation == 3 ? 0 : currentRotation + 1;
                if (CheckBlockFits(currentBlock, nextRotation, currentX, currentY + 1))
                {
                    currentRotation = nextRotation;
                }
            }
        }
    }

    // Move the block down if the force down flag is set.
    // The force down flag is set when the speed count reaches the speed value.
    // The block will move down by 1 line if it fits within the game field.
    // The block will be locked in place if it does not fit within the next line.
    if (forceDown)
    {
        // Reset the speed count
        // The speed count is reset to 0 after the block has moved down by 1 line.
        speedCount = 0;

        // Increase the speed of the game for every 50 blocks placed.
        // The speed of the game is increased by reducing the delay between frames.
        // The more blocks placed, the faster the game will become.
        if (blockCount % 50 == 0 && speed > 10)
        {
            speed--;
        }

        // Check if the current block will fit if the block moves down by 1 line.
        // If it can, then we need to increment the current Y postion of the block.
        // This will move the starting point for that block onto a new line.
        if(CheckBlockFits(currentBlock, currentRotation, currentX, currentY + 1))
        {
            currentY++;
        }
        else
        {
            // The current block does not fit within the next line if it were to move down.
            // In this case, we need to lock the block in place on the screen.
            // This is done by copying the block to the screen buffer.
            for (int tx = 0; tx < 4; tx++)
            {
                for (int ty = 0; ty < 4; ty++)
                {
                    int rotationIndex = Rotate(tx, ty, currentRotation);
                    int screenIndex = (currentY + ty) * screenWidth + (currentX + tx);

                    if (tetrominos[currentBlock][rotationIndex] != '.')
                    {
                        gameField[screenIndex] = currentBlock + 1;
                    }
                }
            }

            // Check for full lines.
            // We need to check if any lines are full after the block has been locked in place.
            // If a line is full, then we need to clear the line and move the lines above down by 1 line.
            // We won't clear the line immediately, but we will mark it for clearing.
            // This is to prevent the line from being cleared before the player can see it.
            // It will also act as a visual cue to the player that a line is full.
            for (int ty = 0; ty < 4; ty++)
            {
                if (currentY + ty < screenHeight)
                {
                    bool isLineFull = true;
                    for (int tx = 1; tx < fieldWidth + 1; tx++)
                    {
                        int screenIndex = (currentY + ty) * screenWidth + tx;
                        if (gameField[screenIndex] == 0)
                        {
                            isLineFull = false;
                            break;
                        }
                    }

                    if (isLineFull)
                    {
                        clearedLines.Add(currentY + ty);

                        for (int tx = 1; tx < fieldWidth + 1; tx++)
                        {
                            int screenIndex = (currentY + ty) * screenWidth + tx;
                            gameField[screenIndex] = 8;
                        }
                    }
                }
            }

            // As the current block is now locked inplace, we need to setup the next block.
            // The next block will become the current block.
            // The new next block will be randomly selected from the 7 Tetrominos.
            currentBlock = nextBlock;
            nextBlock = randmoiser.Next(0, 6);

            // Set the initial position and rotation of the current block.
            // This will be the starting position of the block on the screen.
            // This will be the top-center of the game field.
            currentRotation = 0;
            currentX = 3;
            currentY = 0;

            // Increrment the block count.
            // The block count is used to keep track of the number of blocks placed by the player.
            // The block count is used to calculate the player's score.
            blockCount++;

            // Check if the new block fits within the game field.
            // If the new block does not fit, then the game is over.
            // The game is over if the new block collides with another block at the starting position.
            // The game is over if the new block is outside the game field.
            if (!CheckBlockFits(currentBlock, currentRotation, currentX, currentY))
            {
                isGameOver = true;
            }
        }
    }

    // Draw the current block to the screen
    // The current block is drawn to the screen buffer.
    // The block is drawn as a 4x4 grid of characters.
    for (int tx = 0; tx < 4; tx++)
    {
        for (int ty = 0; ty < 4; ty++)
        {
            int rotationIndex = Rotate(tx, ty, currentRotation);
            int screenIndex = (currentY + ty) * screenWidth + (currentX + tx);

            if (tetrominos[currentBlock][rotationIndex] != '.')
            {
                screen[screenIndex] = currentBlock + 1;
            }
            else if(tetrominos[currentBlock][rotationIndex] == '.')
            {
                // NOTE:
                // This is for testing purposes only.

                // Draw the empty space around the block
                // The empty space will only be drawn if the current value in the screen buffer is 0.
                // This is to prevent overwritting the border of the game field.
                if (isTestMode && screen[screenIndex] == 0)
                {
                    screen[screenIndex] = 10;
                }
            }
        }
    }

    // Check if we have identified any full lines.
    // If we have, we need to briefly highlight the full lines and then clear them.
    if (clearedLines.Count > 0)
    {
        // Increment the line count.
        // We need to keep track of the number of lines cleared in the game.
        // The line count is used to calculate the player's score.
        lineCount += clearedLines.Count;

        // Transfer the contents of the game field to the screen buffer.
        // The game field is a secondary screen buffer that holds the accumulated blocks.
        // This will ensure that any updates to the game field are reflected in the screen buffer when it is next rendered.
        SetGameFieldWithinScreenBuffer(screen, gameField, screenWidth, screenHeight);

        // Render the screen to the console, so that it will highlight the full lines.
        // After the rendering, we will delay the game loop by 400 milliseconds.
        // This is to give the player a chance to see the full lines before they are cleared.
        RenderScreen(screen, screenWidth, screenHeight);
        Task.Delay(400).Wait();

        // Clear the full lines
        // The full lines are cleared by setting the value in each column of the line within the gaemField to 0.
        // The lines above the cleared line are moved down by 1 line.
        // The lines are moved down by copying the line above to the line below.
        // The lines are copied from the bottom of the game field to the top.
        // This is done to prevent overwriting the lines that have not been moved yet.
        for (int lineIndex = 0; lineIndex < clearedLines.Count; lineIndex++)
        {
            int line = clearedLines[lineIndex];
            for (int rowIndex = line; rowIndex > 0; rowIndex--)
            {
                for (int colIndex = 1; colIndex < fieldWidth + 1; colIndex++)
                {
                    int screenIndex = rowIndex * screenWidth + colIndex;
                    int screenIndexAbove = (rowIndex - 1) * screenWidth + colIndex;
                    gameField[screenIndex] = gameField[screenIndexAbove];
                }
            }
        }

        // Transfer the contents of the game field to the screen buffer.
        // The game field is a secondary screen buffer that holds the accumulated blocks.
        // This will ensure that any updates to the game field are reflected in the screen buffer when it is next rendered.
        SetGameFieldWithinScreenBuffer(screen, gameField, screenWidth, screenHeight);
    }

    // Calculate the score.
    // The score is calculated based on the number of blocks cleared, lines cleared, and the current game speed.
    score = CalculateScore(score, blockCount, clearedLines.Count);

    // TODO:
    // Draw the score, level, and next block to the screen.
    SetDetailsWIthinScreenBuffer(score, lineCount, blockCount, nextBlock);

    // Draw the screen to the console.
    // This will draw the contents of screen buffer to the console.
    RenderScreen(screen, screenWidth, screenHeight);
}

// At this point, the game is over.
// We will display a "Game Over!" message to the console.
// The game screen will be cleared and the message will be displayed at the top-left corner.
Console.SetCursorPosition(0, 0);
Console.Clear();

Console.WriteLine("Game Over!");
Console.WriteLine($"Score: {score}");
Console.WriteLine($"Blocks: {blockCount}");
Console.WriteLine($"Lines: {lineCount}");

// Prevent the console window from closing immediately.
// The console window will remain open until a key is pressed.
Console.WriteLine("\nPress any key to exit.");
Console.ReadKey();

// Below are functions and methods used in the game loop.
// These functions are used to calculate positioning, update the game state and draw the screen.
// The functions are called from the game loop to update the game state and draw the screen.

// The SetDetailsWIthinScreenBuffer function is used to display the score, lines, blocks, and next block to the screen.
// The function takes the score, lines, blocks, and next block as arguments.
// The function updates the screen buffer with the details.
// The details are displayed on the right side of the screen.
void SetDetailsWIthinScreenBuffer(int score, int lines, int blocks, int nextBlock)
{
    screen[2 * screenWidth + 13] = 'N';
    screen[2 * screenWidth + 14] = 'E';
    screen[2 * screenWidth + 15] = 'X';
    screen[2 * screenWidth + 16] = 'T';

    for (int tx = 0; tx < 4; tx++)
    {
        for (int ty = 0; ty < 4; ty++)
        {
            int screenIndex = (3 + ty) * screenWidth + (13 + tx);
            int charIndex = ty * 4 + tx;

            screen[screenIndex] = 0;

            if (tetrominos[nextBlock][charIndex] == 'X')
            {
                screen[screenIndex] = nextBlock + 1;
            }
        }
    }

    screen[8 * screenWidth + 13] = 'L';
    screen[8 * screenWidth + 14] = 'I';
    screen[8 * screenWidth + 15] = 'N';
    screen[8 * screenWidth + 16] = 'E';
    screen[8 * screenWidth + 17] = 'S';

    string lineCountText = lines.ToString();
    screen[10 * screenWidth + 13] = lineCountText[0];
    screen[10 * screenWidth + 14] = lines >= 10 ? lineCountText[1] : 0;
    screen[10 * screenWidth + 15] = lines >= 100 ? lineCountText[2] : 0;
    screen[10 * screenWidth + 16] = lines >= 1000 ? lineCountText[3] : 0;
    screen[10 * screenWidth + 17] = lines >= 10000 ? lineCountText[4] : 0;

    screen[12 * screenWidth + 13] = 'B';
    screen[12 * screenWidth + 14] = 'L';
    screen[12 * screenWidth + 15] = 'O';
    screen[12 * screenWidth + 16] = 'C';
    screen[12 * screenWidth + 17] = 'K';
    screen[12 * screenWidth + 18] = 'S';

    string blockCountText = blockCount.ToString();
    screen[14 * screenWidth + 13] = blockCountText[0];
    screen[14 * screenWidth + 14] = blockCount >= 10 ? blockCountText[1] : 0;
    screen[14 * screenWidth + 15] = blockCount >= 100 ? blockCountText[2] : 0;
    screen[14 * screenWidth + 16] = blockCount >= 1000 ? blockCountText[3] : 0;
    screen[14 * screenWidth + 17] = blockCount >= 10000 ? blockCountText[4] : 0;
    screen[14 * screenWidth + 18] = blockCount >= 100000 ? blockCountText[5] : 0;
}

// The SetGameFieldWithinScreenBuffer function is used to copy the contents of the game field to the screen buffer.
// The game field is a secondary screen buffer that holds the accumulated blocks.
void SetGameFieldWithinScreenBuffer(int[] screen, int[] gameField, int width, int height)
{
    for (int index = 0; index < screen.Length; index++)
    {
        int row = index / width % height;
        int col = index % width;

        if ((col > 0 && col < 11) && row < 17)
        {
            screen[index] = gameField[index];
        }
    }
}

// The CalculateScore function is used to calculate the player's score.
// The score is calculated based on the number of blocks cleared and lines cleared.
int CalculateScore(int currentScore, int blocks, int lines)
{
    int score = currentScore;

    if (blocks > 0)
    {
        // The score is calculated based on the number of blocks placed.
        score += blocks * 15;
    }

    if (lines > 0)
    {
        // The score is calculated using a formula that gives more points for clearing multiple lines at once.
        score += (1 << lines) * 50;
    }

    return score;
}

// The RenderScreen function is used to draw the screen buffer to the console.
// The screen buffer is a 1D array of integers.
// Each integer represents a character on the screen.
// The integer value is an index into a character set.
// The character set is " ABCDEFG=#".
void RenderScreen(int[] screen, int width, int height)
{
    // Before we draw the contents of the screen buffer to the console, we need to clear the console.
    // The console is cleared by setting the cursor position to (0, 0).
    // This moves the cursor to the top-left corner of the console.
    // The screen buffer is drawn to the console from that point.
    // By re-drawing to the screen buffer, we can update the screen without flickering.
    // We are not using the Console.Clear() method because it is slow and flickers the screen.
    // The cursor was hidden at the start of the program to prevent it from being displayed on the console.
    Console.SetCursorPosition(0, 0);

    // Draw the screen to the console
    // The screen buffer is drawn to the console.
    // Each integer in the screen buffer is used as an index into the character set.
    // The character at the index is then drawn to the console.
    // The screen is drawn row by row.
    for (int rowIndex = 0; rowIndex < height; rowIndex++)
    {
        for (int colIndex = 0; colIndex < width; colIndex++)
        {
            int screenIndex = rowIndex * screenWidth + colIndex;
            char character = screen[screenIndex] < characterSet.Length
                ? characterSet[screen[screenIndex]]
                : (char)screen[screenIndex];

            // Set the color of the character based on the block type.
            // The color is set by changing the foreground color of the console.
            // The color is set before drawing the character to the console.
            Console.ForegroundColor = screen[screenIndex] switch
            {
                0 => ConsoleColor.Gray,
                1 => ConsoleColor.Red,
                2 => ConsoleColor.Blue,
                3 => ConsoleColor.Green,
                4 => ConsoleColor.Cyan,
                5 => ConsoleColor.Magenta,
                6 => ConsoleColor.Yellow,
                7 => ConsoleColor.DarkYellow,
                9 => ConsoleColor.Gray,
                10 => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };

            // Draw the character to the console.
            Console.Write(character);
        }

        // Move the cursor to the next row
        // After drawing a row of characters, the cursor is moved to the next row.
        Console.Write("\n");
    }
}

// The Tetrominos can be rotated in 4 different ways.
// We will represent the rotation of a Tetromino as an integer value from 0 to 3.
// The integer value will represent the number of 90 degree clockwise rotations.
int Rotate(int tx, int ty, int rotation)
    => (rotation % 4) switch
    {
        0 => ty * 4 + tx,           // 0 degrees
        1 => 12 + ty - (tx * 4),    // 90 degrees
        2 => 15 - (4 * ty) - tx,    // 180 degrees
        3 => 3 - ty + (4 * tx),     // 270 degrees
        _ => throw new NotImplementedException() // Should never happen, but intellicode is asking for it.
    };

// The CheckBlockFits function is used to check if a block will fit within the game field.
// The function takes the Tetromino, rotation, position X, and position Y as arguments.
// The function returns a boolean value indicating if the block fits within the game field.
// The function checks each block of the Tetromino to see if it fits within the game field or if it collides with another block.
bool CheckBlockFits(int tetromino, int rotation, int posX, int posY)
{
    for (int tx = 0; tx < 4; tx++)
    {
        for (int ty = 0; ty < 4; ty++)
        {
            int rotationIndex = Rotate(tx, ty, rotation);
            int screenIndex = (posY + ty) * (screenWidth) + (posX + tx);

            if ((posX + tx) >= 0 && (posX + tx) < screenWidth)
            {
                if (posY + ty >= 0 && posY + ty < screenHeight)
                {
                    if (tetrominos[tetromino][rotationIndex] == 'X' && screen[screenIndex] != 0)
                    {
                        return false;
                    }
                }
            }
        }
    }

    return true;
}