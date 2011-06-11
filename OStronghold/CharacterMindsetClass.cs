﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OStronghold
{
    //this class will portray the different kinds of mindsets(personalities) possible. each different type of personality will
    //cause NPCs to decide and perform different actions to suit their own goal.
    //
    //scale on a 1-100:
    // - money (money over fame and xp) i.e: fight many battles for money, become mercenary for hire
    // - fame (fame over money and xp) i.e: be honorable, fight to become famous, fight well known battles only
    // - xp (xp over fame and money) i.e: fight big battles, train and practice hard, to become skilled
    //
    //i.e: money = 25%
    //     fame = 50%
    //     xp = 25%
    //
    //decision time: 1d100
    //
    //result = 1-25 -> character decides on money option
    //result = 26-75 -> character decides on fame option
    //result = 76-100 -> character decides on xp option

    public class CharacterMindsetClass
    {
        #region Members

        //Attributes for decision making
        public int _moneyScale;
        public int _fameScale;
        public int _xpScale;

        #endregion        

        #region Constructor

        public CharacterMindsetClass()
        {
            _moneyScale = Consts.rand.Next(0, 100);
            _fameScale = Consts.rand.Next(0, 100 - _moneyScale);
            _xpScale = 100 - _moneyScale - _fameScale;
        }

        #endregion

    }
}
