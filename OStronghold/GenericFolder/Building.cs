﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OStronghold.GenericFolder
{
    public class Building
    {
        #region Members       

        private int _buildingID; //building id
        private int _ownerID; //owner id of building
        private int _type; //building type
        private string _name; //name of building
        private Status _hp; //hp of buildling
        private int _costToBuild; //cost to build building for each level
        private Status _level; //level of building
        private Gametime _startBuildTime; //timestamp of when started to build
        private Gametime _endBuildTime; //timestamp of when building finish
        private Consts.buildingState _buildingState; //state of the building

        public int BuildingID
        {
            get { return _buildingID; }
        }
        public int OwnerID
        {
            get { return _ownerID; }
        }
        public int Type
        {
            get { return _type; }
        }
        public string Name
        {
            get { return _name; }
        }
        public Status HP
        {
            get { return _hp; }
        }
        public int CostToBuild
        {
            get { return _costToBuild; }
        }
        public Status Level
        {
            get { return _level; }
        }
        public Gametime StartBuildTime
        {
            get { return _startBuildTime; }
        }
        public Gametime EndBuildTime
        {
            get { return _endBuildTime; }
        }
        public Consts.buildingState BuildingState
        {
            get { return _buildingState; }
            set { _buildingState = value; }
        }


        #endregion

        #region Constructor

        public Building()
        {
        }

        public Building(int buildingIDValue, int ownerIDValue, int typeValue, string nameValue, Status hpValue, int costToBuildValue, Status levelValue, Gametime startBuildTimeValue,
                        Gametime endBuildTimeValue, Consts.buildingState buildStateValue)
        {
            _buildingID = buildingIDValue;
            _ownerID = ownerIDValue;
            _type = typeValue;
            _name = String.Copy(nameValue);
            _hp = new Status(hpValue);            
            _costToBuild = costToBuildValue;
            _level = new Status(levelValue);
            _startBuildTime = new Gametime(startBuildTimeValue);
            _endBuildTime = new Gametime(endBuildTimeValue);
            _buildingState = buildStateValue;
        }

        #endregion

        #region Methods        

        public virtual string getBuildingString()
        {
            return "";
        }

        #endregion

    }
}