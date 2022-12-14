//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1
{
    using System;
    using System.Collections.Generic;
    
    public partial class Appointment
    {
        public int appointment_id { get; set; }
        public string patient_id { get; set; }
        public Nullable<int> treatment_id { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<System.DateTime> appointmentDate { get; set; }
        public Nullable<System.TimeSpan> appointmentTime { get; set; }
        public Nullable<bool> isApproved { get; set; }
        public Nullable<System.DateTime> admitdate { get; set; }
    
        public virtual Patient Patient { get; set; }
        public virtual Treatment Treatment { get; set; }
        public virtual User User { get; set; }
    }
}
