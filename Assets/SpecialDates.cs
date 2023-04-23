using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Calendar;

public class SpecialDates : MonoBehaviour
{
    static Dictionary<string, int> monthStrings = new Dictionary<string, int>();
    List<string> monthDays = new List<string>();

    static void AddEvent(string dayMon, string year, string eventDetails)
    {
        // Set the month
        int month = 0;
        foreach (var mon in monthStrings)
        {
            if (dayMon.Contains(mon.Key))
            {
                month = mon.Value;
            }
        }

        //Set the year
        int yr = Int32.Parse(year);

        // Handles dates that have & in the line from Academic Calendar
        if (dayMon.Contains("&"))
        {
            int index = dayMon.IndexOf("&");
            string num1 = dayMon.Substring(index - 3, 2);
            string num2 = dayMon.Substring(index + 1);

            int day1 = Int32.Parse(num1);
            int day2 = Int32.Parse(num2);

            DateTime date1 = new DateTime(yr, month, day1);
            Calendar.CalendarScript.specialDates.Add(date1, eventDetails);
            DateTime date2 = new DateTime(yr, month, day2);
            Calendar.CalendarScript.specialDates.TryAdd(date2, eventDetails);
            //Debug.Log("Add date: " + month + " " + num1 + ", " + year + " - " + eventDetails);
            //Debug.Log("Add date: " + month + " " + num2 + ", " + year + " - " + eventDetails);
        }
        // Handles dates that have , or - in the line from Academic Calendar
        else if (dayMon.Contains(",") || dayMon.Contains("-"))
        {
            if (dayMon.Contains(","))
            {
                int index = dayMon.IndexOf(",");
                string num1 = dayMon.Substring(index - 2, 2);

                int day = Int32.Parse(num1);

                DateTime date = new DateTime(yr, month, day);
                Calendar.CalendarScript.specialDates.TryAdd(date, eventDetails);
                //Debug.Log("Add date: " + month + " " + num1 + ", " + year + " - " + eventDetails);
            }
            if (dayMon.Contains("-"))
            {
                int index = dayMon.IndexOf("-");

                string num1 = dayMon.Substring(index - 2, 2);
                int begNum = Int32.Parse(num1);

                string num2 = dayMon.Substring(index + 1);
                int endNum = Int32.Parse(num2);

                for (int i = begNum; i <= endNum; i++)
                {
                    DateTime date = new DateTime(yr, month, i);
                    Calendar.CalendarScript.specialDates.TryAdd(date, eventDetails);
                    //Debug.Log("Add date: " + month + " " + i + ", " + year + " - " + eventDetails);
                }
            }
        }
        // Regular dates listed on the Academic Calendar
        else
        {
            int index = dayMon.IndexOf(" ");
            string num1 = dayMon.Substring(index + 1);

            int day = Int32.Parse(num1);

            DateTime date = new DateTime(yr, month, day);
            Calendar.CalendarScript.specialDates.TryAdd(date, eventDetails);
            //Debug.Log("Add date: " + month + " " + num1 + ", " + year + " - " + eventDetails);
        }        
    }

    private void Start()
    {
        StartCoroutine(GetRequest("https://prescott.erau.edu/campus-life/academic-calendar"));
        
        // Populate the month dictionary with full month names
        monthStrings.Add("January", 1);
        monthStrings.Add("February", 2);
        monthStrings.Add("March", 3);
        monthStrings.Add("April", 4);
        monthStrings.Add("May", 5);
        monthStrings.Add("June", 6);
        monthStrings.Add("July", 7);
        monthStrings.Add("August", 8);
        monthStrings.Add("September", 9);
        monthStrings.Add("October", 10);
        monthStrings.Add("November", 11);
        monthStrings.Add("December", 12);

        // Populate the month dictionary with abbreviated month names
        monthStrings.Add("Jan.", 1);
        monthStrings.Add("Feb.", 2);
        monthStrings.Add("Mar.", 3);
        monthStrings.Add("Apr.", 4);
        //no abbreviation for May
        monthStrings.Add("Jun.", 6);
        monthStrings.Add("Jul.", 7);
        monthStrings.Add("Aug.", 8);
        monthStrings.Add("Sept.", 9);
        monthStrings.Add("Oct.", 10);
        monthStrings.Add("Nov.", 11);
        monthStrings.Add("Dec.", 12);
    }


    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }

        string webpage = uwr.downloadHandler.text;
        var lines = webpage.Split('\n');
        string year = "";

        //determine if it's a pertinent line, ignore lines before
        bool semesterInfo = false;
        bool monthFound = false;
        string monthDaySave = "";
        string eventSave = "";
        foreach (var line in lines)
        {
            if (line.Contains("Semester"))
            {
                semesterInfo = true;
                if (line.Contains("Fall"))
                {
                    year = line.Substring(9, 4);
                    //Debug.Log("line="+line+" year="+year);
                }
                else if (line.Contains("Spring"))
                {
                    year = line.Substring(11, 4);
                    //Debug.Log("line="+line+" year="+year);
                }
            }
            else if ((semesterInfo == true) && (monthFound == false))
            {
                if (line.Contains("td"))
                {
                    //Debug.Log(line);
                    //parse out part between > <
                    var monthDayLines = line.Split('<', '>');
                    if (monthDayLines.Length > 2)
                    {
                        string data = monthDayLines[2]; //the second line contains the date (in the way it's split)
                                                        //Debug.Log(monthDayLines[2]);
                        data = data.Replace("&nbsp;", " ");
                        data = data.Replace("&amp;", "&");


                        string pattern = "^[A-Z][a-z][a-z][a-z]?[.]";  //month abbreviated Feb.
                        string pattern2 = "^[A-Z][a-z]+ [0-9]+";  //March 8
                        Regex rg = new Regex(pattern);
                        Regex rg2 = new Regex(pattern2);
                        if (rg.IsMatch(data))
                        {
                            //Debug.Log("Month found:"+data);
                            monthFound = true;
                            monthDaySave = data;
                        }
                        else if (rg2.IsMatch(data))
                        {
                            //Debug.Log("Month found:"+data);
                            monthFound = true;
                            monthDaySave = data;
                        }
                    }
                }
            }
            else if ((semesterInfo == true) && (monthFound == true))
            {

                //Debug.Log(line);
                //parse out part between >le <
                var monthDayLines = line.Split('<', '>');
                //Debug.Log("2="+monthDayLines[2]+" 4="+monthDayLines[4]);
                if (monthDayLines.Length > 2)
                {
                    string data = monthDayLines[2];
                    if ((data.Length < 2) && (monthDayLines.Length > 4))
                    { //for the <a>Commencement</a>
                        data = monthDayLines[4];
                    }
                    data = data.Replace("&ldquo;", "\"");
                    data = data.Replace("&rdquo;", "\"");
                    data = data.Replace("&amp;", "&");

                    string pattern = "[^><]+";
                    Regex rg = new Regex(pattern);
                    if (rg.IsMatch(data))
                    {
                        //Debug.Log("data="+data);
                        if (data.Length > 1)
                        {
                            if (eventSave == "")
                            {
                                eventSave = data;
                            }
                            else
                            {
                                eventSave = eventSave + "\n" + data;
                            }
                            //Debug.Log("event found:"+data+" len(data)"+data.Length);

                        }
                    }

                }
                // }
                if (line.Contains("/td>"))
                {
                    //Debug.Log("EVENT: "+monthDaySave+" - "+eventSave);
                    AddEvent(monthDaySave, year, eventSave);
                    monthDaySave = "";
                    eventSave = "";
                    monthFound = false;
                }

            }
        } //end foreach

        Calendar.CalendarScript.specialDatesLoaded = 1;
    }
}
