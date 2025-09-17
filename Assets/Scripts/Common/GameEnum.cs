using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnum
{
    public enum CardRarity
    {
        INVALID = -1,
        BRONZE,
        SILVER,
        GOLD,
        LEGENDARY,
        GOD,
        MAX
    }

    public enum Class
    {
        INVALID = -1,
        NEUTRAL,
        FOREST,
        SWORD,
        RUNA,
        DRAGON,
        ABYSS,
        HAVEN,
        PORTAL,
        MAX
    }

    public enum  CardType
    {
        INVALID = -1,
        FOLLOWER,
        SPELL,
        AMULET,
        MAX
    }

    public enum BGM
    {
        TITLE = 0,
        MAIN,
        MAX
    }

    public enum SE
    {

        MAX
    }
}
