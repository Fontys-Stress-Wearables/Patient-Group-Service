﻿using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Models;

public class Patient
{
    public string Id { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime Birthdate { get; set; }
    public bool IsActive { get; set; } = false;
    public virtual ICollection<PatientGroupPatient> PatientGroupPatients { get; set; } = new List<PatientGroupPatient>();
    public virtual Organization Organization { get; set; }
}
