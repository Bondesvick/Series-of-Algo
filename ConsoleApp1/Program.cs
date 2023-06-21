using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using System;

using System.Text.RegularExpressions;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string GetRecipient(string message, int position)
{
    var pattern = "@[A-Za-z0-9-_]*";
    var matches = Regex.Matches(message, pattern);
    var found = matches.ToList()[position - 1].ToString();
    // Your code goes here

    return found != null ? found[1..] : "";
}

Console.WriteLine(GetRecipient("@JoeBloggs yo", 1));

WebClient client = new WebClient();
string s = client.DownloadString("https://coderbyte.com/api/challenges/logs/web-logs-raw");
string pattern = "\\?shareLinkId=.{12}";
//Console.WriteLine(s);

var matches = Regex.Matches(s, pattern);

//foreach (Match match in Regex.Matches(s, pattern, RegexOptions.IgnoreCase))
//    Console.WriteLine("{0} (duplicates '{1}') at position {2}",
//                          match.Value, match.Groups[1].Value, match.Index);

var found = matches.Select(x => x.Value.Substring(x.Value.Length - 12)).ToList();
//Console.WriteLine(string.Join('-', found));

//Console.WriteLine(found.Count());

var grouped = found.GroupBy(found => found);

var final = "";

foreach (var group in grouped)
{
    final += group.Count() > 1 ? (group.Key + ":" + group.Count() + "\n") : (group.Key + "\n");
}

//Console.WriteLine(final);

//Console.WriteLine(string.Join("-", groupe)) 3:45
//////////////////////////////////////////////////////////////////////////////////////////////
///
///
///
using (HttpClient client4 = new HttpClient())
{
    string url1 = "https://coderbyte.com/api/challenges/json/age-counting";
    HttpResponseMessage response4 = await client4.GetAsync(url1);

    if (response4.IsSuccessStatusCode)
    {
        string json = await response4.Content.ReadAsStringAsync();
        int count2 = RemoveOnesAndCountKeys(json);
        Console.WriteLine("Number of keys in modified JSON object: " + count2);
    }
    else
    {
        Console.WriteLine("GET request failed with status code: " + response4.StatusCode);
    }
}

static int RemoveOnesAndCountKeys(string json)
{
    JObject jsonObject = JObject.Parse(json);
    string data = (string)jsonObject["data"];

    string[] items = data.Split(',');
    int count = 0;

    foreach (string item in items)
    {
        if (item.Contains("age=1"))
        {
            string[] keyValue = item.Trim().Split('=');
            string key = keyValue[0].Trim();
            string age = keyValue[1].Trim();

            if (age == "1")
            {
                jsonObject.Remove(key);
            }
        }
        else if (item.Contains("age="))
        {
            count++;
        }
    }

    return count;
}
///////////////////////////////////////////////////////////////////////////////////////////
///

HttpClient client3 = new HttpClient();

// Send a GET request and retrieve the response
HttpResponseMessage response = client3.GetAsync("https://coderbyte.com/api/challenges/json/age-counting").Result;

// Read the response content as a string
string responseContent = response.Content.ReadAsStringAsync().Result;

// Parse the response as JSON and extract the "data" value
dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
string dat = jsonObject.data;

// Split the data string into individual items
string[] items = dat.Split(',');

// Count the items with age equal to or greater than 50
int count = 0;
foreach (string item in items)
{
    if (item.Contains("age="))
    {
        int age = int.Parse(item.Split('=')[1]);
        if (age >= 50)
        {
            count++;
        }
    }
}

Console.WriteLine(count);

////////////////////////////////////////////////////////////////////////////////

string url = "https://coderbyte.com/api/challenges/json/json-cleaning";
HttpClient client2 = new HttpClient();

try
{
    string json = await client2.GetStringAsync(url);
    Console.WriteLine(json);
    JToken data = JToken.Parse(json);

    CleanObject(data);

    string modifiedJson = data.ToString();
    Console.WriteLine(modifiedJson);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}

static void CleanObject(JToken token)
{
    if (token.Type == JTokenType.Object)
    {
        var properties = token.Children<JProperty>().ToList();
        foreach (var property in properties)
        {
            CleanObject(property.Value);
            if (IsInvalidValue(property.Value))
                property.Remove();
        }
    }
    else if (token.Type == JTokenType.Array)
    {
        var array = (JArray)token;
        for (int i = array.Count - 1; i >= 0; i--)
        {
            CleanObject(array[i]);
            if (IsInvalidValue(array[i]))
                array.RemoveAt(i);
        }
    }
}

static bool IsInvalidValue(JToken token)
{
    if (token.Type == JTokenType.Null || token.Type == JTokenType.Undefined)
        return true;

    if (token.Type == JTokenType.String)
    {
        string value = token.Value<string>();
        return string.IsNullOrEmpty(value) || value == "N/A" || value == "-";
    }

    return false;
}