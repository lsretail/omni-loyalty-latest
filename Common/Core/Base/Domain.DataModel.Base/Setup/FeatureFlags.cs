using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LSRetail.Omni.Domain.DataModel.Base.Setup
{
    public class FeatureFlags
    {
        [DataMember]
        public List<FeatureFlag> Flags { get; set; } = new List<FeatureFlag>();

        public void AddFlag(FeatureFlagName flagName, string flagValue)
        {
            Flags.Add(new FeatureFlag()
            {
                name = flagName,
                value = flagValue
            });
        }

        public void AddFlag(FeatureFlagName flagName, int flagValue)
        {
            Flags.Add(new FeatureFlag()
            {
                name = flagName,
                value = flagValue.ToString()
            });
        }

        public bool GetFlagBool(FeatureFlagName flagName)
        {
            FeatureFlag flag = Flags.Find(f => f.name == flagName);
            if (flag == null)
                throw new LSOmniException(StatusCode.NoEntriesFound, string.Format("Flag {0} not found", flagName));

            try
            {
                return Convert.ToInt16(flag.value) == 1;
            }
            catch
            {
                return Convert.ToBoolean(flag.value);
            }
        }

        public int GetFlagInt(FeatureFlagName flagName)
        {
            FeatureFlag flag = Flags.Find(f => f.name == flagName);
            if (flag == null)
                throw new LSOmniException(StatusCode.NoEntriesFound, string.Format("Flag {0} not found", flagName));

            try
            {
                return Convert.ToInt32(flag.value);
            }
            catch
            {
                return 0;
            }
        }

        public string GetFlagString(FeatureFlagName flagName)
        {
            FeatureFlag flag = Flags.Find(f => f.name == flagName);
            if (flag == null)
                throw new LSOmniException(StatusCode.NoEntriesFound, string.Format("Flag {0} not found", flagName));

            return (flag.value == null) ? string.Empty : flag.value;
        }
    }

    public class FeatureFlag
    {
        public FeatureFlagName name = FeatureFlagName.None;
        public string value = string.Empty;
    }

    public enum FeatureFlagName
    {
        None,
        AllowAutoLogoff,
        AutoLogOffAfterMin,
        AllowOffline,
        ExitAfterEachTransaction,
        SendReceiptInEmail,
        ShowNumberPad,
        UseLoyaltySystem
    }
}
