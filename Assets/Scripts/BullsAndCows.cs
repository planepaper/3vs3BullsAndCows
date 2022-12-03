using System;

public class BullsAndCows
{
    public int[] GetRandomNumber()
    {
        int[] nums = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        while (nums[0] == 0)
        {
            KnuthShuffle<int>(ref nums);
        }
        int[] chosenNum = new int[4];
        Array.Copy(nums, chosenNum, 4);

        return chosenNum;
    }

    public void KnuthShuffle<T>(ref T[] array)
    {
        System.Random random = new System.Random();
        for (int i = 0; i < array.Length; i++)
        {
            int j = random.Next(array.Length);
            T temp = array[i]; array[i] = array[j]; array[j] = temp;
        }
    }

    public int[] GuessNumber(string guess, int[] num)
    {
        char[] guessed = guess.ToCharArray();
        int strikeCount = 0, ballCount = 0;

        if (guessed.Length != 4)
        {
            Console.WriteLine("Not a valid guess.");
            return new int[] { -1, -1 };
        }

        int bullsCount = 0;
        int cowsCount = 0;

        for (int i = 0; i < 4; i++)
        {
            int curguess = (int)char.GetNumericValue(guessed[i]);
            if (curguess < 1 || curguess > 9)
            {
                Console.WriteLine("Digit must be ge greater 0 and lower 10.");
                return new int[] { -1, -1 };
            }
            if (curguess == num[i])
            {
                bullsCount++;
            }
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    if (curguess == num[j])
                        cowsCount++;
                }
            }
        }

        if (bullsCount == 4)
        {
            Console.WriteLine("Congratulations! You have won!");
            return new int[] { bullsCount, cowsCount };
        }
        else
        {
            Console.WriteLine("Your Score is {0} bulls and {1} cows", bullsCount, cowsCount);
            return new int[] { bullsCount, cowsCount };
        }
    }
}
