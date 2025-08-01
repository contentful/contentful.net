﻿using System;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;

namespace Contentful.Core.Models;

public class BaseSystemProperties
{
    /// <summary>
    /// The unique identifier of the resource.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The type of link. Will be null for non link types.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string LinkType { get; set; }

    /// <summary>
    /// The type of the resource.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The link to the environment of the resource.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ContentfulEnvironment Environment { get; set; }

    /// <summary>
    /// The link to the space of the resource.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Space Space { get; set; }

    /// <summary>
    /// The date and time the resource was created. Will be null when not applicable, e.g. for arrays.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// The link to the user that created this content. Will only be present for management API call.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public User CreatedBy { get; set; }

    /// <summary>
    /// The date and time the resource was last updated. Will be null when not applicable or when the resource has never been updated.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// The link to the user that last updated this content. Will only be present for management API call.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public User UpdatedBy { get; set; }
        
    /// <summary>
    /// The current version of the resource. Will only be present for management API calls. 
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Version { get; set; }
}