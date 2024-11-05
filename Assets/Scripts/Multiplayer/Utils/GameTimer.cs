//*************************************************************************************************************
/*  Game Timer
 *  A subclass of the countdown timer
 *      Modifies it to present the timer better (mm:ss)
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 08/08/23 Improved timer system
 */
//*************************************************************************************************************

public class GameTimer : CountdownTimer
{
    protected override string _GetTimerString(int time)
    {
        int minutesLeft = (int)(time / 60f);
        // Remainder
        int secondsLeft = time % 60;
        return $"{minutesLeft.ToString().PadLeft(2, '0')}:" +
               $"{secondsLeft.ToString().PadLeft(2, '0')}";
    }
}
