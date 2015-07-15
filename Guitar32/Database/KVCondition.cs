using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar32.Data;

namespace Guitar32.Database
{
    public class KVCondition : KV<string, object>
    {

        public const string OR = "OR";
        public const string AND = "AND";
        public const string NONE = "";

        private bool _hasTilde;
        private string _conditionLink;
        

        /// <summary>
        /// If this KVCondition has tilde or none
        /// </summary>
        public bool HasTilde
        {
            get
            {
                return _hasTilde;
            }
        }

        /// <summary>
        /// Condition link of this KVCondition, otherwise, NONE
        /// </summary>
        public string ConditionLink
        {
            get
            {
                return _conditionLink;
            }
        }

        /// <summary>
        /// Construct new instance of KVCondition
        /// </summary>
        public KVCondition()
            : base()
        {

        }

        /// <summary>
        /// Construct new instance of KVCondition
        /// </summary>
        /// <param name="key">The key of this KVCondition</param>
        /// <param name="value">The value of this KVCondition</param>
        /// <param name="hasTilde">(Optional) Boolean value if key in this condition has tilde</param>
        public KVCondition(string key, string value, bool hasTilde = true) : base(key, value)
        {
            this._hasTilde = hasTilde;
            this._conditionLink = NONE;
        }

        /// <summary>
        /// Construct new instance of KVCondition
        /// </summary>
        /// <param name="key">The key of this KVCondition</param>
        /// <param name="value">The value of this KVCondition</param>
        /// <param name="conditionLink">(Optional) Condition linking string value</param>
        public KVCondition(string key, string value, string conditionLink) : base(key, value)
        {
            this._hasTilde = true;
            this._conditionLink = conditionLink;
        }

        /// <summary>
        /// Construct new instance of KVCondition
        /// </summary>
        /// <param name="key">The key of this KVCondition</param>
        /// <param name="value">The value of this KVCondition</param>
        /// <param name="conditionLink">(Optional) Condition linking string value</param>
        /// <param name="hasTilde">(Optional) Boolean value if key in this condition has tilde</param>
        public KVCondition(string key, string value, string conditionLink, bool hasTilde)
            : base(key, value)
        {
            this._conditionLink = conditionLink;
            this._hasTilde = hasTilde;
        }

        /// <summary>
        /// Get the condition string
        /// </summary>
        /// <returns></returns>
        public string GetCondition()
        {
            return string.Format(HasTilde ? "`{0}` = {1}" : "{0} = {1}", Key, Value);
        }

        /// <summary>
        /// Check if this KVCondition has 
        /// </summary>
        /// <returns></returns>
        public bool HasConditionLink()
        {
            return ConditionLink != KVCondition.NONE;
        }
        

    }
}
