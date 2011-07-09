﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using OStronghold.CharacterFolder;
using OStronghold.GenericFolder;
using OStronghold.StrongholdFolder;

namespace OStronghold
{
    public class Gametime
    {
        #region Members

        private long _lastgametick;
        private long _elapsedticks;

        private int _minute;
        private int _day;
        private int _hour;

        public long LastGameTick
        {
            get { return _lastgametick; }
            set { _lastgametick = value; }
        }
        public long ElapsedTicks
        {
            get { return _elapsedticks; }
            set { _elapsedticks = value; }
        }

        public int Minute
        {
            get { return _minute; }
            set 
            { 
                _minute = value;
                while (_minute >= 60)
                {
                    incOneHour();
                    _minute -= 60;
                }
            }
        }
        public int Day
        {
            get { return _day; }
            set { _day = value; }
        }
        public int Hour
        {
            get { return _hour; }
            set 
            {                
                _hour = value;
                while (_hour >= 24)
                {                                       
                    Day++;
                    _hour -= 24;
                }                
            }

        }

        #endregion

        #region Constructor

        public Gametime()
        {
            _lastgametick = DateTime.Now.Ticks;
            _gametickPassedEvent += this.OnGameTickPassedHandler;
            _anHourPassedEvent += this.OnHourPassedEventHandler;
            _aDayPassedEvent += this.OnDayPassedEventHandler;
        }        

        public Gametime(int dayvalue, int hourvalue)
        {
            _lastgametick = DateTime.Now.Ticks;
            Minute = 0;
            Day = dayvalue;
            Hour = hourvalue;
            _anHourPassedEvent += this.OnHourPassedEventHandler;
            _gametickPassedEvent += this.OnGameTickPassedHandler;
            _aDayPassedEvent += this.OnDayPassedEventHandler;
        }

        public Gametime(int minvalue)
        {
            _lastgametick = DateTime.Now.Ticks;
            Minute = minvalue;//automatically updates the day value in the hour encapsulation
            _anHourPassedEvent += this.OnHourPassedEventHandler;
            _gametickPassedEvent += this.OnGameTickPassedHandler;
            _aDayPassedEvent += this.OnDayPassedEventHandler;
        }

        public Gametime(int dayvalue, int hourvalue, int minutevalue)
        {
            _lastgametick = DateTime.Now.Ticks;
            Minute = minutevalue;
            Day = dayvalue;
            Hour = hourvalue;
            _anHourPassedEvent += this.OnHourPassedEventHandler;
            _gametickPassedEvent += this.OnGameTickPassedHandler;
            _aDayPassedEvent += this.OnDayPassedEventHandler;
        }

        public Gametime(Gametime target)
        {
            this.CopyGameTime(target);
        }

        #endregion

        #region Methods

        public void CopyGameTime(Gametime target)
        {
            _minute = target.Minute;
            _day = target.Day;
            _hour = target.Hour;
        }

        public int getTotalHours()
        {
            return ((_day * 24) + _hour);
        }

        public int getTotalMinutes()
        {
            return (getTotalHours() * 60);
        }

        public int compareTimeOnly(Gametime targetTime)
        {
            int thisTotalMinutes = this._hour * 60 + this._minute;
            int targetTimeTotalMinutes = targetTime._hour * 60 + targetTime._minute;

            if (thisTotalMinutes == targetTimeTotalMinutes) return 0;
            else if (thisTotalMinutes > targetTimeTotalMinutes) return -1;
            else return 1;
        }//compares the times of the both gametime objects and return 0 if equal, -1 if this object is greater , 1 if targetTime is greater

        public void incOneDay()
        {
            Day++;
        }

        public void incXMinutes(int minutes)
        {
            Minute += minutes;
            OnGameTickPassed(System.EventArgs.Empty);
        }

        private void incOneHour()
        {
            Hour++;            
        }

        public bool isMidnight()
        {
            return (_hour == 0 && _minute == 0);            
        }

        #endregion

        #region Operators overload

        public override string ToString()
        {
            return ("Day " + _day + " Hour " + _hour + " Minutes " + _minute);
        }

        public static Gametime operator +(Gametime t1, Gametime t2)
        {
            return (new Gametime(t1.Day + t2.Day, t1.Hour + t2.Hour, t1.Minute + t2.Minute));
        }

        public static Gametime operator +(Gametime t1, int minutes)
        {
            return (new Gametime(t1.getTotalMinutes() + minutes));
        }

        public static Gametime operator -(Gametime t1, Gametime t2)
        {
            int t1Minutes = t1.getTotalMinutes();
            int t2Minutes = t2.getTotalMinutes();

            if (t1Minutes >= t2Minutes)
            {
                return new Gametime(t1Minutes - t2Minutes);
            }
            else return new Gametime(t2Minutes - t1Minutes); 

        }

        public static bool operator ==(Gametime t1, Gametime t2)
        {
            return (t1.Day == t2.Day && t1.Hour == t2.Hour && t1.Minute == t2.Minute);
        }

        public static bool operator !=(Gametime t1, Gametime t2)
        {
            return !(t1 == t2);
        }

        public static bool operator >=(Gametime t1, Gametime t2)
        {
            return (t1.getTotalMinutes() >= t2.getTotalMinutes());
        }

        public static bool operator <(Gametime t1, Gametime t2)
        {
            return !(t1 >= t2);
        }

        public static bool operator <=(Gametime t1, Gametime t2)
        {
            return (t1.getTotalMinutes() <= t2.getTotalMinutes());
        }

        public static bool operator >(Gametime t1, Gametime t2)
        {
            return !(t1 <= t2);
        }


        public override bool Equals(object obj)
        {
            if (obj is Gametime)
            {
                return (this == (Gametime)obj);
            }
            return false;
        }             

        #endregion

        #region Events

        public delegate void gametickPassedHandler(object sender, EventArgs e);
        public event gametickPassedHandler _gametickPassedEvent;

        public delegate void hourPassedHandler(object sender, EventArgs e);
        public event hourPassedHandler _anHourPassedEvent;

        public delegate void dayPassedHandler(object sender, EventArgs e);
        public event dayPassedHandler _aDayPassedEvent;

        protected virtual void OnGameTickPassed(System.EventArgs e)
        {
            if (_gametickPassedEvent != null)
            {
                _gametickPassedEvent(this, e);
            }
        }

        public void OnGameTickPassedHandler(object sender, EventArgs e)
        {
            Consts.writeEnteringMethodToDebugLog(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            LinkedList<int> numberLinkList = new LinkedList<int>();
            int[] commonerUpdateOrder = new int[Program._aStronghold._commoners.Count];
            int index, buildingID;
            InventoryItem item;
            Job job;
            Character person;

            #region updateStrongholdLeaderDecisionMaker

            if (Program._aStronghold._leader._decisionmaker.listOfActionsToDo.Count != 0)
            {
                bool notEnoughFunds = false;
                int buildResults = -1;

                foreach (ActionsToDo todo in Program._aStronghold._leader._decisionmaker.listOfActionsToDo)
                {
                    #region build to do action
                    if (todo._action == action.Build)
                    {
                        #region build hut todo
                        if (todo._objectTypeID == Consts.hut)
                        {
                            Consts.globalEvent.writeEvent("Stronghold leader orders to build a hut.", Consts.eventType.Stronghold, Consts.EVENT_DEBUG_NORMAL);
                            if (todo._parameters == parameters.None)
                            {
                                buildResults = Program._aStronghold.buildHut(Consts.hut_buildtime);
                            }
                            else if (todo._parameters == parameters.BuildImmediate)
                            {
                                buildResults = Program._aStronghold.buildHut(Consts.immediatebuildtime);
                            }
                        }
                        #endregion
                        #region build granary todo
                        else if (todo._objectTypeID == Consts.granary)
                        {
                            Consts.globalEvent.writeEvent("Stronghold leader orders to build a granary.", Consts.eventType.Stronghold, Consts.EVENT_DEBUG_NORMAL);
                            if (todo._parameters == parameters.None)
                            {
                                buildResults = Program._aStronghold.buildGranary(Consts.granary_buildtime);
                            }
                            else if (todo._parameters == parameters.BuildImmediate)
                            {
                                buildResults = Program._aStronghold.buildGranary(Consts.immediatebuildtime);
                            }
                        }
                        #endregion
                        #region build farm todo
                        else if (todo._objectTypeID == Consts.farm)
                        {
                            Consts.globalEvent.writeEvent("Stronghold leader orders to build a farm.", Consts.eventType.Stronghold, Consts.EVENT_DEBUG_NORMAL);
                            if (todo._parameters == parameters.None)
                            {
                                buildResults = Program._aStronghold.buildFarm(Consts.farm_buildtime);
                            }
                            else if (todo._parameters == parameters.BuildImmediate)
                            {
                                buildResults = Program._aStronghold.buildFarm(Consts.immediatebuildtime);
                            }
                        }
                        #endregion
                        #region build employmentoffice todo
                        else if (todo._objectTypeID == Consts.employmentoffice)
                        {
                            Consts.globalEvent.writeEvent("Stronghold leader orders to build an employment office.", Consts.eventType.Stronghold, Consts.EVENT_DEBUG_NORMAL);
                            if (todo._parameters == parameters.None)
                            {
                                buildResults = Program._aStronghold.buildEmploymentOffice(Consts.employmentoffice_buildtime);
                            }
                            else if (todo._parameters == parameters.BuildImmediate)
                            {
                                buildResults = Program._aStronghold.buildEmploymentOffice(Consts.immediatebuildtime);
                            }
                        }
                        #endregion
                        if (buildResults == -1)
                        {
                            notEnoughFunds = true;
                        }
                    }
                    #endregion
                }
                Program._aStronghold._leader._decisionmaker.listOfActionsToDo.Clear();
                Program._aStronghold._leader._decisionmaker.listOfPhenomenons.Clear();

                if (notEnoughFunds)
                {
                    Program._aStronghold._leader._decisionmaker.insertPhenomenon(Consts.stronghold, Consts.stronghold_treasury, subobject.Capacity, behaviour.Empty,parameters.None);
                }

            }//there are some actions to do

            #endregion

            #region randomize update order for characters
            //create commonerUpdateOrderArray
            for (int x = 1; x <= Program._aStronghold._commoners.Count; x++)
            {
                numberLinkList.AddLast(x);
            }//build the link list so we can randomize off it.

            for (int x = 0; x < Program._aStronghold._commoners.Count; x++)
            {
                index = Consts.rand.Next(0, numberLinkList.Count);
                commonerUpdateOrder[x] = numberLinkList.ElementAt(index);
                numberLinkList.Remove(commonerUpdateOrder[x]);
            }//picking randomly from numberlist and populating the order in the array 
            //-
            #endregion

            #region update character actions

            for (int x = 0; x < Program._aStronghold._commoners.Count; x++)
            {
                person = ((Character)Program._aStronghold._commoners[commonerUpdateOrder[x]]); //goes through commoner order list

                if (person._health.hp.HealthState == Consts.healthState.Alive)
                {

                    job = Program._aStronghold.searchJobByID(person._jobID);

                    if (person._characterActions.Count > 0)
                    {
                        person._currentActionFinishTime = person._characterActions.Peek().FinishTime;
                    }
                    else
                    {
                        //FATAL ERROR! Character action should have minimum Idle action.
                        Consts.globalEvent.writeEvent("FATAL ERROR!" + person._name + " (" + person._id + ") has no actions in queue. Needs at least Idle action.", Consts.eventType.Stronghold, Consts.EVENT_DEBUG_MIN);
                    }

                    if (person._currentActionFinishTime > Program._gametime)
                    {
                        //wait
                    }//person is doing something - wait until action is over
                    else //person is finished doing action, do results
                    {
                        switch (person._characterActions.Peek().Action)
                        {
                            #region buyingfood
                            case Consts.characterGeneralActions.BuyingFood:
                                if (person._characterPreviousActions.Last()._action == Consts.characterGeneralActions.BuyingFood)
                                {
                                    
                                }//if person has been trying to buy food twice in a row then have the 
                                else
                                {
                                    LinkedList<Building> granaries = Program._aStronghold.searchBuildingsByType(Consts.granary);
                                    InventoryItem food;
                                    int totalFood = 0;
                                    int granaryIDToBuy = -1;

                                    if (granaries.Count != 0)
                                    {
                                        foreach (BuildingWithJobsAndInventory granary in granaries)
                                        {
                                            if (granary.BuildingState == Consts.buildingState.Built)
                                            {
                                                food = granary.searchInventoryByID(Consts.FOOD_ID);
                                                if (food != null)
                                                {
                                                    totalFood += food.Quantity;
                                                    if (granary.InventoryCapacity.Current != 0)
                                                    {
                                                        granaryIDToBuy = granary.BuildingID;
                                                    }
                                                }
                                            }
                                        }

                                        if (person._characterinventory.searchForItemByName(Consts.GOLD_NAME).Quantity == 0)
                                        {
                                            person._characterActions.Dequeue();
                                            Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") has no money to buy " + Consts.FOOD_NAME + ".", Consts.eventType.Character, Consts.EVENT_DEBUG_NORMAL);
                                        }
                                        else if (totalFood == 0)
                                        {
                                            person._characterActions.Dequeue();
                                            Consts.globalEvent.writeEvent("There are no food in the granaries for " + person._name + " (" + person._id + ") to buy.", Consts.eventType.Stronghold, Consts.EVENT_DEBUG_NORMAL);
                                        }
                                        else
                                        {
                                            if (granaryIDToBuy != -1)
                                            {
                                                person.buyFood(granaryIDToBuy);
                                                person._characterActions.Dequeue();
                                                Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") finished buying " + Consts.FOOD_NAME + ".", Consts.eventType.Character, Consts.EVENT_DEBUG_NORMAL);
                                            }
                                            //else no granaries were found                                        
                                        }//else buy food from granaryIDTobuy                                    
                                    }
                                    else
                                    {
                                        person._characterActions.Dequeue();
                                        Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") cannot buy any " + Consts.FOOD_NAME + " since there are no granaries.", Consts.eventType.Character, Consts.EVENT_DEBUG_NORMAL);
                                    }
                                    person.addNewPreviousAction(Consts.characterGeneralActions.BuyingFood);
                                }
                                break;
                            #endregion
                            #region eating
                            case Consts.characterGeneralActions.Eating:
                                person._bodyneeds.HungerState = Consts.hungerState.JustAte;
                                person._bodyneeds.LastAteTime.CopyGameTime(Program._gametime);
                                person._characterActions.Dequeue(); //action is finished                                 
                                Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") finished eating.", Consts.eventType.Character, Consts.EVENT_DEBUG_NORMAL);
                                person.addNewPreviousAction(Consts.characterGeneralActions.Eating);
                                break;
                            #endregion
                            #region lookingforplacetolive
                            case Consts.characterGeneralActions.LookingForPlaceToLive:
                                if (person._homeID == Consts.stronghold_yard)
                                {
                                    buildingID = person.findPlaceToLive();
                                    person._homeID = buildingID;
                                    person._locationID = buildingID;
                                    Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") finished looking for a place to live.", Consts.eventType.Character, Consts.EVENT_DEBUG_MAX);
                                }//person is looking for place to live, returns STRONGHOLD_YARD if does not find any
              
                                if (person._homeID != Consts.stronghold_yard)
                                {
                                    person._characterActions.Dequeue();
                                }//person found place to live
                                else
                                {
                                    
                                    //do nothing
                                }//do not dequeue the action and remains for next update - person did not find any accomondations
                                person.addNewPreviousAction(Consts.characterGeneralActions.LookingForPlaceToLive);
                                break;
                            #endregion
                            #region sleeping
                            case Consts.characterGeneralActions.Sleeping:
                                person._bodyneeds.SleepState = Consts.sleepState.Awake;
                                person._bodyneeds.LastSleptTime.CopyGameTime(Program._gametime);
                                person._characterActions.Dequeue();
                                Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") finished sleeping.", Consts.eventType.Character, Consts.EVENT_DEBUG_NORMAL);
                                person.addNewPreviousAction(Consts.characterGeneralActions.Sleeping);
                                break;
                            #endregion
                            #region working
                            case Consts.characterGeneralActions.Working:
                                person._characterActions.Dequeue();
                                item = person._characterinventory.retrieveItemInInventory(null, Consts.GOLD_ID);
                                if (item != null)
                                {
                                    item.Quantity += job.Payroll;
                                    person._characterinventory.putInInventory(item);
                                }//person has gold
                                else
                                {
                                    person._characterinventory.putInInventory(new InventoryItem(Consts.GOLD_NAME, Consts.GOLD_ID, Consts.GOLD_WEIGHT, job.Payroll));
                                }//person doesn't have gold, put gold into inventory        
                                Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") finished working.", Consts.eventType.Character, Consts.EVENT_DEBUG_NORMAL);
                                if (String.Compare(job.JobName, Consts.builderName) == 0)
                                {
                                    Building building;
                                    Program._aStronghold.searchBuildingByID(job.BuildingID,out building);

                                    if (building != null)
                                    {
                                        building.NumberOfCurrentBuilders--;
                                        job.BuildingID = Consts.stronghold_yard;
                                    }
                                }//builder stops working , need to take builder off building
                                person.addNewPreviousAction(Consts.characterGeneralActions.Working);
                                break;
                            #endregion
                        }
                    }

                    #region person current action
                    if (person._characterActions.Count > 0)
                    {
                        if (person._characterActions.Peek().Action == Consts.characterGeneralActions.Idle)
                        {
                            Gametime finishTime;
                            #region Hunger check
                            //Check if hungry
                            if (person._bodyneeds.HungerState == Consts.hungerState.JustAte)
                            {
                                person._bodyneeds.HungerState = Consts.hungerState.Full;
                                person._health.hp.Regeneration = 1;
                            }//just ate last round, this round character is full
                            else if (person._bodyneeds.HungerState == Consts.hungerState.Full &&
                                Program._gametime >= person._bodyneeds.LastAteTime + (int)Consts.hungerTimer.Normal)
                            {
                                person._bodyneeds.HungerState = Consts.hungerState.Normal;
                                person._health.hp.Regeneration = 0;
                            }//pass normal hunger time and loses hp regeneration
                            else if (person._bodyneeds.HungerState == Consts.hungerState.Normal &&
                                Program._gametime > person._bodyneeds.LastAteTime + (int)Consts.hungerTimer.Full)
                            {
                                person._bodyneeds.HungerState = Consts.hungerState.Hungry;
                            }//pass hungry hunger time
                            else if (person._bodyneeds.HungerState == Consts.hungerState.Hungry)
                            {
                                Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") is hungry.", Consts.eventType.Character, Consts.EVENT_DEBUG_NORMAL);
                                person._bodyneeds.HungerState = Consts.hungerState.Hungry;
                                person._health.hp.Regeneration = -1;
                            }//if hungry and then remains hungry and loses hp every game tick
                            #endregion
                            #region Sleep check                            
                            if (Program._gametime > person._bodyneeds.LastSleptTime + (int)Consts.sleepTimer.Awake)
                            {
                                Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") is sleeping.", Consts.eventType.Character, Consts.EVENT_DEBUG_NORMAL);
                                person._bodyneeds.SleepState = Consts.sleepState.MustSleep;
                                finishTime = Program._gametime + Consts.actionsData[(int)Consts.characterGeneralActions.Sleeping]._actionDuration;
                                person._characterActions.insertItemIntoQueue(new CharacterAction(Consts.characterGeneralActions.Sleeping, Consts.actionsData[(int)Consts.characterGeneralActions.Sleeping]._actionPriority, finishTime));
                            }
                            #endregion
                            #region Employment check
                            if (person._jobID == -1) //&& person wants to look for job)
                            {
                                LinkedList<Job> listOfAvailableJobs = Program._aStronghold.getAllAvailableJobs();
                                if (listOfAvailableJobs.Count > 0)
                                {
                                    Job availableJob = new Job();
                                    
                                    if (Program._aStronghold.hasAtLeastOneBuilderEmployed())
                                    {
                                        //randomly picks an available job
                                        availableJob = listOfAvailableJobs.ElementAt(Consts.rand.Next(0, listOfAvailableJobs.Count - 1));
                                    }//if there is at least one builder in stronghold
                                    else
                                    {
                                        availableJob = Program._aStronghold.searchFirstAvailableJobByName(Consts.builderName);
                                    }//choose builder job for applying
                                                                        
                                    Building building = new Building();
                                    Program._aStronghold.searchBuildingByID(availableJob.BuildingID, out building);

                                    if (building.BuildingState == Consts.buildingState.Built)
                                    {
                                        person.applyForJob(availableJob.JobID);
                                    }//building that is providing with the job must be built first
                                }//there are jobs in stronghold
                                else
                                {
                                    if (!Program._aStronghold.farmsHasAvailableJobs() && !Program._aStronghold.hasEnoughPlannedOrConstruction(Consts.farm))
                                    {
                                        Consts.globalEvent.writeEvent("Stronghold leader recognizes the need for jobs.", Consts.eventType.Stronghold, Consts.EVENT_DEBUG_NORMAL);
                                        Program._aStronghold._leader._decisionmaker.insertPhenomenon(Consts.stronghold, Consts.stronghold_jobs, subobject.Capacity, behaviour.Empty, parameters.None); //no jobs
                                    }//if there are no available jobs in the farms and no farms are currently planned or under construction
                                }//no available jobs in stronghold
                            }//person applies for first job in the list
                            else
                            {
                                if (job != null)
                                {
                                    if (Program._gametime >= job.StartDate && Program._gametime < job.EndDate)
                                    {
                                        if (Program._gametime.compareTimeOnly(job.StartTime) <= 0 && //gametime >= job start time
                                            Program._gametime.compareTimeOnly(job.EndTime) > 0) //gametime <= job end time
                                        {
                                            bool needToWork = true;
                                            if (String.Compare(job.JobName, Consts.builderName) == 0)
                                            {
                                                LinkedList<Building> allPlannedBuildings = Program._aStronghold.searchBuildingsByBuildingState(Consts.buildingState.UnderConstruction);
                                                if (allPlannedBuildings.Count == 0)
                                                {
                                                    person._currentActionFinishTime.CopyGameTime(Program._gametime);
                                                    needToWork = false;

                                                }//no buildings to build so finish working
                                                else if (job.BuildingID != allPlannedBuildings.First.Value.BuildingID)
                                                {
                                                    allPlannedBuildings.First.Value.NumberOfCurrentBuilders++;
                                                    job.BuildingID = allPlannedBuildings.First.Value.BuildingID;
                                                }//assign builder to building only if the builder is not already assigned
                                            }//if person is a builder

                                            if (needToWork)
                                            {
                                                Gametime workTimeRelative = new Gametime(Program._gametime.Day, job.EndTime.Hour, job.EndTime.Minute);
                                                if (job.EndDate >= workTimeRelative)
                                                {
                                                    finishTime = workTimeRelative;
                                                }
                                                else finishTime = job.EndDate;
                                                Consts.globalEvent.writeEvent(person._name + " (" + person._id + ") is working in his job (" + person._jobID + ").", Consts.eventType.Character, Consts.EVENT_DEBUG_NORMAL);
                                                person._characterActions.insertItemIntoQueue(new CharacterAction(Consts.characterGeneralActions.Working, Consts.actionsData[(int)Consts.characterGeneralActions.Working]._actionPriority, finishTime));
                                            }
                                        }//gametime is between job start time and end time
                                    }//job position is open     
                                    else
                                    {
                                        Consts.globalEvent.writeEvent("The " + job.JobName + " (" + job.JobID + ") has just expired.", Consts.eventType.Stronghold, Consts.EVENT_DEBUG_MIN);
                                        Program._aStronghold._allJobs.Find(job).Value.JobStatus = Consts.JobStatus.Closed;
                                        person._jobID = -1;
                                    }//job is no longer available - taken off the market
                                }//person is already employed
                            }//person already employed or doesn't want to work
                            #endregion
                            #region Accomondation check

                            if (person._locationID == Consts.stronghold_yard && person._homeID == Consts.stronghold_yard)
                            {
                                finishTime = Program._gametime;
                                person._characterActions.insertItemIntoQueue(new CharacterAction(Consts.characterGeneralActions.LookingForPlaceToLive, Consts.actionsData[(int)Consts.characterGeneralActions.LookingForPlaceToLive]._actionPriority, finishTime));
                            }

                            #endregion
                        }
                        else if (person._characterActions.Peek().Action == Consts.characterGeneralActions.Eating)
                        {
                            person._health.staminaUsedThisTick = 3;
                        }
                        else if (person._characterActions.Peek().Action == Consts.characterGeneralActions.LookingForPlaceToLive)
                        {
                            person._health.staminaUsedThisTick = 5;
                        }
                        else if (person._characterActions.Peek().Action == Consts.characterGeneralActions.Sleeping)
                        {
                            person._locationID = person._homeID;
                            //sleeping
                        }
                        else if (person._characterActions.Peek().Action == Consts.characterGeneralActions.Working)
                        {
                            person._health.staminaUsedThisTick = 10;
                            person._locationID = job.BuildingID;
                            if (String.Compare(job.JobName, Consts.builderName) == 0)
                            {
                                LinkedList<Building> allPlannedBuildings = Program._aStronghold.searchBuildingsByBuildingState(Consts.buildingState.UnderConstruction);
                                if (allPlannedBuildings.Count == 0)
                                {
                                    person._currentActionFinishTime.CopyGameTime(Program._gametime);
                                }//no buildings to build so finish working
                            }
                        }
                    }
                    #endregion

                    #region person health update
                    //person health is updated at the end of each tick
                    person._health.hp.Current += person._health.hp.Regeneration;
                    person._health.mp.Current += person._health.mp.Regeneration;
                    person._health.stamina.Current = person._health.stamina.Current + person._health.stamina.Regeneration - person._health.staminaUsedThisTick;
                    person._health.staminaUsedThisTick = 0; //reset the stamina usage at the end of gametick
                    #endregion
                }//if person is alive then update actions
            }

            #endregion

            #region update building actions

            int totalFoodProduced = 0;
            foreach (Building building in Program._aStronghold._buildingsList)
            {
                #region building construction phases

                if (building.BuildingState != Consts.buildingState.Built)
                {
                    //update man working hours
                    building.NumberOfManBuildingHoursLeft -= building.NumberOfCurrentBuilders;

                    //update building constructor status
                    if ((building.NumberOfManBuildingHoursLeft < 0 && building.BuildingState != Consts.buildingState.Built))
                    {
                        building.BuildingState = Consts.buildingState.Built;
                        Consts.globalEvent.writeEvent("Construction of a " + building.Name + " has been finished.", Consts.eventType.Building, Consts.EVENT_DEBUG_MIN);
                    }//building is finished
                    else if (Program._gametime >= building.StartBuildTime && building.NumberOfManBuildingHoursLeft >= 0) //&& building.BuildingState != Consts.buildingState.UnderConstruction)
                    {
                        building.BuildingState = Consts.buildingState.UnderConstruction;
                        Consts.globalEvent.writeEvent("The " + building.Name + " is being constructed. " + building.NumberOfManBuildingHoursLeft + " man working hours left.", Consts.eventType.Building, Consts.EVENT_DEBUG_MIN);
                    }//building is underconstruction                
                }//update building phases only if the building is not built
                #endregion
                
                if (Program._gametime.isMidnight())
                {
                    if (building.Type == Consts.farm && building.BuildingState == Consts.buildingState.Built)
                    {                        
                        int[] jobList = ((BuildingWithJobsAndInventory)building).Jobs;
                        for (int i = 0; i < jobList.Length; i++)
                        {
                            if (null != jobList && jobList[i] > 0)
                            {
                                if (Program._aStronghold.searchJobByID(jobList[i]).JobStatus == Consts.JobStatus.Taken)
                                {
                                    totalFoodProduced += Consts.numberOfFoodProducedPerFarmer;
                                }//job is taken and hence producing food
                            }//job list has jobs
                        }
                    }//transfer farm foods to granary at end of the day                
                }//occurs at the end of each day                
            }

            bool notthrown = false;            
            InventoryItem foodTransferredToGranary = new InventoryItem(Consts.FOOD_NAME, Consts.FOOD_ID, Consts.FOOD_WEIGHT, totalFoodProduced);
            int foodLeftAfterTransfer = totalFoodProduced;
            if (totalFoodProduced > 0)
            {
                foreach (Building building in Program._aStronghold._buildingsList)
                {
                    if (building.Type == Consts.granary && !notthrown && building.BuildingState == Consts.buildingState.Built)
                    {
                        BuildingWithJobsAndInventory granary = building as BuildingWithJobsAndInventory;
                        if (granary.hasEnoughStorageSpace())
                        {
                            //Building result;
                            
                            //Program._aStronghold.searchBuildingByID(granary.BuildingID, out result);

                            if (granary.InventoryCapacity.Max - granary.InventoryCapacity.Current >= foodTransferredToGranary.Quantity)
                            {
                                foodLeftAfterTransfer = 0;
                                notthrown = true;
                            }//no surplus of food left after transfer
                            else
                            {
                                foodLeftAfterTransfer = foodTransferredToGranary.Quantity - (granary.InventoryCapacity.Max - granary.InventoryCapacity.Current);                                
                            }//surplus of food , need to throw some out

                            granary.addToInventory(foodTransferredToGranary);

                            Consts.writeToDebugLog(totalFoodProduced - foodLeftAfterTransfer + " food transferred to granary.");
                            Consts.globalEvent.writeEvent(totalFoodProduced - foodLeftAfterTransfer + " food transferred to " + granary.Name + " (" + granary.BuildingID + "). Granary capacity is: " + granary.InventoryCapacity.Current + "/" + granary.InventoryCapacity.Max, Consts.eventType.Building, Consts.EVENT_DEBUG_MIN);
                        }//granary has enough room to store
                    }
                }
                if (!notthrown)
                {                    
                    //food wasted
                    Consts.globalEvent.writeEvent(foodLeftAfterTransfer + " food was thrown away.", Consts.eventType.Building, Consts.EVENT_DEBUG_NORMAL);
                    Consts.writeToDebugLog("Food not transferred - " + foodLeftAfterTransfer + " wasted.");

                    if (!Program._aStronghold.hasEnoughPlannedOrConstruction(Consts.granary))
                    {
                        Consts.globalEvent.writeEvent("Stronghold leader recognizes the need for a granary.", Consts.eventType.Stronghold, Consts.EVENT_DEBUG_NORMAL);
                        Program._aStronghold._leader._decisionmaker.insertPhenomenon(Consts.stronghold, Consts.granary, subobject.Existence, behaviour.Empty,parameters.None);
                    }
                }
            }//transfer food only if food produced is > 0
            Consts.writeExitingMethodToDebugLog(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endregion
        }//actions to do in every game tick


        protected virtual void OnHourPassed(System.EventArgs e)
        {
            if (_anHourPassedEvent != null)
            {
                _anHourPassedEvent(this, e);
            }
        }

        public void OnHourPassedEventHandler(object sender, EventArgs e)
        {
            
        }//every hour passed must perform actions       

        protected virtual void OnDayPassed(System.EventArgs e)
        {
            if (_aDayPassedEvent != null)
            {
                _aDayPassedEvent(this, e);
            }
        }

        public void OnDayPassedEventHandler(object sender, EventArgs e)
        {

        }//every day passed must perform actions       
        #endregion
    }//game time
}
