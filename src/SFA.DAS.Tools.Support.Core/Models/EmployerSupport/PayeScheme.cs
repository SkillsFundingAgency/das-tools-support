﻿using System;

namespace SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

public class PayeScheme
{
    public string DasAccountId { get; set; } = "";
    public string Ref { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime AddedDate { get; set; }
    public DateTime? RemovedDate { get; set; }
    public string HashedPayeRef { get; set; } = "";
    public string ObscuredPayeRef { get; set; } = "";
    public string PayeRefWithOutSlash
    {
        get
        {
            return (!string.IsNullOrEmpty(Ref)) ? Ref.Replace("/", string.Empty) : string.Empty;
        }
    }
}

