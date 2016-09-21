using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace at.mschwaig.mped.definitions
{
    [Table("EXPERIMENTS")]
    public class Experiment
    {
        [Key]
        public int Id { get; private set; }

        public string Name { get; private set; }

        public Experiment(string name)
        {
            this.Name = name;
        }
    }
}
