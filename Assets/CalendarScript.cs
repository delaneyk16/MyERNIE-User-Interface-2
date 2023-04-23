using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;  //needed for TMP
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading;


namespace Calendar
{
    public class CalendarScript : MonoBehaviour
    {
        public class Day
        {
            public int dayNum;
            public Color dayColor;
            public GameObject obj;

            public Day(int dayNum, Color dayColor, GameObject obj)
            {
                this.dayNum = dayNum;
                this.dayColor = dayColor;
                this.obj = obj;
                UpdateColor(dayColor);
                UpdateDay(dayNum);
            }

            public void UpdateColor(Color newColor)
            {
                obj.GetComponent<Image>().color = newColor;
                dayColor = newColor;
            }

            public void UpdateDay(int newDayNum)
            {
                //print("update day for " + newDayNum);
                this.dayNum = newDayNum;
                if (dayColor == Color.white || dayColor == Color.green)
                {
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = (dayNum + 1).ToString();
                    //return dayNum + 1; //debugging
                }
                else
                {
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    //return 0; //debugging
                }
            }
        } //end of Day class

        public List<Day> days = new List<Day>();
        public static Dictionary<DateTime, string> specialDates = new Dictionary<DateTime, string>(); //stores dates and events from Academic Calendar, populated in SpecialDates2.cs
        public static int specialDatesLoaded = 0;

        public Transform[] weeks; //stores CalendarWeeks

        public TextMeshProUGUI MonthAndYear; //public Text MonthAndYear;
        public static DateTime currDate = DateTime.Now;

        public GameObject popUp;
        public TextMeshProUGUI eventDesc;
        public TextMeshProUGUI date;


        private void Start()
        {
            UpdateCalendar(DateTime.Now.Year, DateTime.Now.Month);
        }

        private void Update()
        {
            if (specialDatesLoaded == 1)
            {
                UpdateCalendar(DateTime.Now.Year, DateTime.Now.Month);
                specialDatesLoaded = 2;
            }
        }

        void UpdateCalendar(int year, int month)
        {
            DateTime temp = new DateTime(year, month, 1);
            currDate = temp;
            MonthAndYear.text = temp.ToString("MMMM") + " " + temp.Year.ToString();
            int startDay = GetMonthStartDay(year, month);
            int endDay = GetTotalNumberOfDays(year, month);

            days.Clear();

            //print("****startdate " + startDay + " endDay" + endDay);
            for (int w = 0; w < 6; w++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Day newDay;
                    int currDay = (w * 7) + i;
                    //print("currDay " + currDay);
                    if (currDay < startDay || ((currDay - startDay) >= endDay))
                    {
                        //print("gray!!!!");
                        newDay = new Day(currDay - startDay, Color.grey, weeks[w].GetChild(i).gameObject);
                    }
                    else
                    {
                        //print("white");
                        newDay = new Day(currDay - startDay, Color.white, weeks[w].GetChild(i).gameObject);
                    }
                    days.Add(newDay);
                }
            }

            if (DateTime.Now.Year == year && DateTime.Now.Month == month)
            {
                days[(DateTime.Now.Day - 1) + startDay].UpdateColor(Color.yellow);
            }

            foreach (DateTime sd in specialDates.Keys)
            {
                if (sd.Month == month && sd.Year == year)
                {
                    Color lightBlue = new Color(0.4f, 0.5f, 0.9f);
                    days[(sd.Day - 1) + startDay].UpdateColor(lightBlue);
                }
            }
        }

        int GetMonthStartDay(int year, int month)
        {
            DateTime temp = new DateTime(year, month, 1);
            return (int)temp.DayOfWeek;
        }

        int GetTotalNumberOfDays(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        public void SwitchMonth(int direction)
        {
            if (direction < 0)
            {
                currDate = currDate.AddMonths(-1);
            }
            else
            {
                currDate = currDate.AddMonths(1);
            }
            UpdateCalendar(currDate.Year, currDate.Month);
        }

        // Function that runs when a day is clicked on
        public void ToggleOn(int day)
        {
            int dayNum = days[day].dayNum + 1;
            Color dayColor = days[day].dayColor;

            if (dayColor != Color.grey)
            {
                popUp.SetActive(true);
                date.GetComponent<TextMeshProUGUI>().text = "";
                eventDesc.GetComponent<TextMeshProUGUI>().text = "";
                printEvent(dayNum);
            }
        }

        // Function that runs when the X is clicked
        public void ToggleOff()
        {
            popUp.SetActive(false);
        }

        // Function that prints the events happening on the date
        public void printEvent(int day)
        {
            string monthStr = currDate.ToString("MMMM");
            string displayedDate = monthStr + " " + day;
            date.GetComponent<TextMeshProUGUI>().text = displayedDate;

            int month = currDate.Month;
            int year = currDate.Year;

            DateTime d = new DateTime(year, month, day);

            if (specialDates.ContainsKey(d))
            {
                string events = specialDates[d];
                eventDesc.GetComponent<TextMeshProUGUI>().text = events;
                //Debug.Log("This is a special date");
            }
            else
            {
                eventDesc.GetComponent<TextMeshProUGUI>().text = "There are no events happening.\n";
            }
        }
    }
}