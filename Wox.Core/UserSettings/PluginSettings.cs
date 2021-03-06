﻿using System.Collections.Generic;

namespace Wox.Core.UserSettings
{

    public class PluginSettings
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public List<string> ActionKeywords { get; set; }

        public bool Disabled { get; set; }
    }
}
