﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace at.mschwaig.mped.persistence
{
    public class Experiment
    {
        public int ExperimentId { get; private set; }

        [StringLength(60)]
        [Index(IsUnique = true)]
        public string Name { get; private set; }

        public virtual List<Problem> Problems { get; set; }

        // Used by Entity Framework
        private Experiment() { }

        public Experiment(string name)
        {
            this.Name = name;
        }
    }
}
