using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Calendar;

public class SpecialDates : MonoBehaviour
{
    Dictionary<string, int> monthStrings = new Dictionary<string, int>();

    void Start()
    {
        // Scrape through the Academic Calendar page on the ERAU website
        StartCoroutine(GetRequest("https://prescott.erau.edu/campus-life/academic-calendar"));

        // Populate the month dictionary
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
    }


    IEnumerator GetRequest(string uri)
    {
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
            string year = "";
            List<string> monthDays = new List<string>();

            foreach (var line in lines)
            {
                // Handle the year from the HTML text
                if (line.Contains("Semester"))
                {
                    if (line.Contains("Fall"))
                    {
                        year = line.Substring(9, 4);
                    }
                    else if (line.Contains("Spring"))
                    {
                        year = line.Substring(11, 4);
                    }
                }

                // Pick out the lines that have dates in them
                foreach (var mon in monthStrings.Keys)
                {
                    if (line.Contains(mon + " "))
                    {
                        var monthDayLines = line.Split('<', '>');
                        string monthDay = monthDayLines[2]; //the second line contains the date (in the way it's split)

                        // Handling special cases within the HTML text and populate the monthDays list
                        if (monthDay.Contains("&"))
                        {
                            int index = monthDay.IndexOf("&");
                            string num1 = monthDay.Substring(index - 3, 2);
                            string num2 = monthDay.Substring(index + 6);

                            monthDays.Add(mon + num1);
                            monthDays.Add(mon + num2);
                        }
                        else if (monthDay.Contains(",") || monthDay.Contains("-"))
                        {
                            if (monthDay.Contains(","))
                            {
                                int index = monthDay.IndexOf(",");
                                string num1 = monthDay.Substring(index - 2, 2);

                                monthDays.Add(mon + num1);
                            }
                            if (monthDay.Contains("-"))
                            {
                                int index = monthDay.IndexOf("-");

                                string num1 = monthDay.Substring(index - 2, 2);
                                int begNum = Int32.Parse(num1);

                                string num2 = monthDay.Substring(index + 1);
                                int endNum = Int32.Parse(num2);

                                for (int i = begNum; i <= endNum; i++)
                                {
                                    monthDays.Add(mon + i);
                                }
                            }
                        }
                        else
                        {
                            int index = monthDay.IndexOf(" ");

                            string num = monthDay.Substring(index + 1);

                            monthDays.Add(mon + num);
                        }
                    }
                }
            }

            // Iterating through the monthDays list to pick out the month and day
            foreach (string x in monthDays)
            {
                string noSpaces = x.Replace(" ", "");
                
                foreach (var mon in monthStrings)
                {
                    if (noSpaces.Contains(mon.Key))
                    {
                        string d = noSpaces.Substring(mon.Key.Length);
                        int day = Int32.Parse(d);

                        int month = mon.Value;

                        int yr = Int32.Parse(year);

                        // Add them to the list of specialDates in the CalendarScript.cs so they will be colored differently
                        DateTime date = new DateTime(yr, month, day);
                        Calendar.CalendarScript.specialDates.Add(date);
                    }
                }
            }
        }
    }
}
