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
        public static List<DateTime> specialDates = new List<DateTime>(); //stores dates that have events from Academic Calendar
        public static int specialDatesLoaded = 0;
        Dictionary<string, string> monthNicknames = new Dictionary<string, string>();

        public Transform[] weeks; //stores CalendarWeeks

        public TextMeshProUGUI MonthAndYear; //public Text MonthAndYear;
        public static DateTime currDate = DateTime.Now;

        public GameObject popUp;
        public TextMeshProUGUI eventDesc;
        public TextMeshProUGUI date;


        private void Start()
        {
            UpdateCalendar(DateTime.Now.Year, DateTime.Now.Month);
            monthNicknames.Add("January", "Jan.");
            monthNicknames.Add("February", "Feb.");
            monthNicknames.Add("March", "Mar.");
            monthNicknames.Add("April", "Apr.");
            monthNicknames.Add("May", "May");
            monthNicknames.Add("June", "Jun.");
            monthNicknames.Add("July", "Jul.");
            monthNicknames.Add("August", "Aug.");
            monthNicknames.Add("September", "Sept.");
            monthNicknames.Add("October", "Oct.");
            monthNicknames.Add("November", "Nov.");
            monthNicknames.Add("December", "Dec.");
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

            foreach (DateTime sd in specialDates)
            {
                if (sd.Month == month)
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
                StartCoroutine(GetRequest("https://prescott.erau.edu/campus-life/academic-calendar", dayNum));
            }
        }

        // Function that runs when the X is clicked
        public void ToggleOff()
        {
            popUp.SetActive(false);
        }





        




        // Web scraping function for the ERAU Academic Calendar
        IEnumerator GetRequest(string uri, int dayNumber)
        {
            


            //Debug.Log("In GetRequest with dayNumber " + dayNumber);
            UnityWebRequest uwr = UnityWebRequest.Get(uri);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }

            else
            {
                string webpage = uwr.downloadHandler.text;
                var lines = webpage.Split('\n');
                bool found = false;
                int printedHours = 0;


                string searchMonth = currDate.ToString("MMMM");
                string monthNickname = "";
                monthNicknames.TryGetValue(searchMonth, out monthNickname);
                //Debug.Log("Month: " + monthNickname);

                date.GetComponent<TextMeshProUGUI>().text = searchMonth + " " + dayNumber;

                // Finds matching date that the user clicks on and gets the event description from the next HTML line
                foreach (var line in lines)
                {
                    if (line.Contains(searchMonth) || line.Contains(monthNickname))
                    {
                        // Special case for the holiday Juneteenth
                        if (line.Contains("Juneteenth"))
                        {
                            eventDesc.GetComponent<TextMeshProUGUI>().text = "Holiday - Juneteenth";
                            break;
                        }

                        //Debug.Log("Line: " + line);
                        var monthDayLines = line.Split('<', '>');
                        string monthDay = monthDayLines[2].Replace("&nbsp;", " ");
                        Debug.Log("monthDay: " + monthDay);

                        if (monthDay.Contains("&"))
                        {
                            int index = monthDay.IndexOf("&");
                            string num1 = monthDay.Substring(index - 3, 2);
                            string num2 = monthDay.Substring(index + 6);

                            // Remove spaces for comparison
                            num1 = num1.Replace(" ", "");
                            num2 = num2.Replace(" ", "");

                            if (num1 == dayNumber.ToString() || num2 == dayNumber.ToString())
                            {
                                found = true;
                            }
                        }
                        else if (monthDay.Contains(",") || monthDay.Contains("-"))
                        {
                            if (monthDay.Contains(","))
                            {
                                int index = monthDay.IndexOf(",");
                                string num1 = monthDay.Substring(index - 2, 2);

                                // Remove spaces before comparison
                                num1 = num1.Replace(" ", "");

                                if (num1 == dayNumber.ToString())
                                {
                                    found = true;
                                    Debug.Log("December 10 special case is complete");
                                }
                            }
                            if (monthDay.Contains("-"))
                            {
                                int index = monthDay.IndexOf("-");
                                string num1 = monthDay.Substring(index - 2, 2);
                                string num2 = monthDay.Substring(index + 1);
                                int number1 = Int32.Parse(num1);
                                int number2 = Int32.Parse(num2);

                                if (dayNumber >= number1 && dayNumber <= number2)
                                {
                                    found = true;
                                }
                            }
                        }
                        else
                        {
                            
                            int index = monthDay.IndexOf(" ");
                            string num = monthDay.Substring(index + 1);

                            // Remove spaces before comparison
                            num = num.Replace(" ", "");

                            // Ensures that date is exact match to HTML text
                            if (num == dayNumber.ToString())
                            {
                                Debug.Log("MonthDay: " + monthDay);
                                found = true;
                            }
                        }
                    }

                    else if (found)
                    {
                        Debug.Log("Line: " + line);
                        var text = line.Split('>', '<');
                        Debug.Log("text[2]: " + text[2]);


                        // Fixing double quote issue from HTML text
                        if (text[2].ToLower().Contains('&'))
                        {
                            string correctText = text[2].Replace("&ldquo;", "\"").Replace("&rdquo;", "\"").Replace("&amp;", "&");
                            eventDesc.GetComponent<TextMeshProUGUI>().text = correctText;
                        }
                        else if (text[2] == "")
                        {
                            eventDesc.GetComponent<TextMeshProUGUI>().text = "Cannot display event.\n\n" +
                                "Please visit prescott.erau.edu/campus-life/academic-calendar for more info.";
                        }
                        else
                        {
                            eventDesc.GetComponent<TextMeshProUGUI>().text = text[2];
                        }

                        break;
                    }
                }

                // If the date is not listed on the Academic Calendar
                if (!found)
                {
                    eventDesc.GetComponent<TextMeshProUGUI>().text = "There are no events happening on this day. ";
                }
            }
        }
    }
}