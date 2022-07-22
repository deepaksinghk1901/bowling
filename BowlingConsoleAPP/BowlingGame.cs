using System;
using System.Collections.Generic;
using System.Linq;
public class BowlingGame
{
    private class Frame
    {
        public int FirstBowl { get; set; } = 0;

        public int SecondBowl { get; set; } = 0;

        public int Score => FirstBowl + SecondBowl;

        public bool IsSpare => (Score == 10) && (FirstBowl != 10);

        public bool IsStrike => FirstBowl == 10;

        public bool IsClosed { get; set; } = false;

        public Frame(int firstBowl) => FirstBowl = firstBowl;

    }

    private readonly List<Frame> rolls = new List<Frame>();

    private bool isFirstBowl = true;

    public void Roll(int pins)
    {

        if ((pins < 0) || (pins > 10))
        {
            throw new ArgumentException($"Score '{pins}' is not in range [0,10]."); 
        }



        if ((rolls.Count == 11) && rolls[9].IsSpare)
        {
            throw new ArgumentException("Cannot roll after bonus roll for spare.");
        }



        if ((rolls.Count == 11) && rolls[10].IsClosed && !rolls[10].IsStrike)
        {
            throw new ArgumentException("Cannot roll after bonus rolls for strike.");
        }



        if ((rolls.Count == 10) && rolls[9].IsClosed && !rolls[9].IsSpare && !rolls[9].IsStrike)
        {
            throw new ArgumentException("Cannot roll if game already has ten frames.");
        }

        if (isFirstBowl)
        {
            rolls.Add(new Frame(pins));
            isFirstBowl = pins == 10;
            rolls.Last().IsClosed = pins == 10;
        }
        else
        {
            var last = rolls.Last();
            if ((last.FirstBowl + pins) > 10)
            {
                throw new ArgumentException("Two rolls in a frame cannot score more than 10 points.");
            }
            last.SecondBowl = pins;
            last.IsClosed = true;
            isFirstBowl = true;
        }
    }
    public int? Score()
    {
        if (rolls.Count == 0)
        {
            throw new ArgumentException("An unstarted game cannot be scored.");
        }
        if (rolls.Count < 10)
        {
            throw new ArgumentException("An incomplete game cannot be scored.");
        }
        if ((rolls.Count == 10) && (rolls.Last().IsStrike || rolls.Last().IsSpare))
        {
            throw new ArgumentException("Bonus rolls for a strike in the last frame must be rolled before score can be calculated.");
        }
        var score = 0;
        Frame frame;
        for (var i = 0; i < 10; ++i)
        {
            frame = rolls[i];
            if (frame.IsStrike)
            {
                score += 10 + GetStrikeBonus(i);
            }
            else if (frame.IsSpare)
            {
                score += 10 + rolls[i + 1].FirstBowl;
            }
            else
            {
                score += frame.Score;
            }
        }
        return score;
    }
    private int GetStrikeBonus(int roll)
    {
        var bonus = rolls[roll + 1].FirstBowl;
        if (bonus == 10)
        {
            if (rolls.Count <= roll + 2)
            {
                throw new ArgumentException("Not enough rolls to compute strike bonus.");
            }
            bonus += rolls[roll + 2].FirstBowl;
        }
        else
        {
            bonus += rolls[roll + 1].SecondBowl;
        }
        return bonus;
    }
}