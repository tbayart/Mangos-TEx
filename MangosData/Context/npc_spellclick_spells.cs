//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MangosData.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class npc_spellclick_spells
    {
        public long npc_entry { get; set; }
        public long spell_id { get; set; }
        public int quest_start { get; set; }
        public bool quest_start_active { get; set; }
        public int quest_end { get; set; }
        public byte cast_flags { get; set; }
        public int condition_id { get; set; }
    }
}
