﻿using System.Collections;
using CarbonAware.Model;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Newtonsoft.Json;

namespace CarbonAware.Plugins.JsonReaderPlugin;

public class CarbonAwareJsonReaderPlugin : ICarbonAware
{
    private readonly ILogger<CarbonAwareJsonReaderPlugin> _logger;


    public CarbonAwareJsonReaderPlugin(ILogger<CarbonAwareJsonReaderPlugin> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<EmissionsData>> GetEmissionsDataAsync(IDictionary props)
    {
        var data = GetSampleJson();

        return await Task.Run(() => GetFilteredData(data, props));
    }

    private IEnumerable<EmissionsData> GetFilteredData(IEnumerable<EmissionsData> data, IDictionary props) {
        var l = props[CarbonAwareConstants.Locations] as IEnumerable<string>;
        List<String> locations = l !=null ? l.ToList() : new List<string>();

        var s = props[CarbonAwareConstants.Start];
        var start = DateTime.Now;
        if (s != null)
        {
            DateTime.TryParse(s.ToString(), out start);
        }
        var e = props[CarbonAwareConstants.End];
        
        // DateTime? end = e != null ? DateTime.Parse(new string(e.ToString())) : null;
        var d = props[CarbonAwareConstants.Duration];
        int durationMinutes =  d!= null ? (int)d : 0;
        
        if (locations.Any()) 
        {
            data = data.Where(ed => locations.Contains(ed.Location));
        }

        if (e != null)
        {
            DateTime end;
            DateTime.TryParse(e.ToString(), out end);
            data = data.Where(ed => ed.TimeBetween(start, end));  // no need to convert to List
        }
        else
        {
            data  = data.Where(ed => ed.Time <= start);
        }

        if (data.Count() != 0)
        {
            data.MaxBy(ed => ed.Time);
        }

        return data;
    }

    private string ReadFromResource(string key)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using Stream streamMetaData = assembly.GetManifestResourceStream(key) ?? throw new NullReferenceException("StreamMedataData is null");
        using StreamReader readerMetaData = new StreamReader(streamMetaData);
        return readerMetaData.ReadToEnd();
    }
 
    protected virtual List<EmissionsData> GetSampleJson()
    {
        var data = ReadFromResource("CarbonAware.Plugins.JsonReaderPlugin.test-data-azure-emissions.json");
        var jsonObject = JsonConvert.DeserializeObject<EmissionsJsonFile>(data);
        return jsonObject.Emissions;
    }
}
