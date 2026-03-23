namespace TestUpscaleApp.Models
{
    public class User
    {
        public long Id { get; set; }
        
        public required string Names { get; set; }
        
        public required string FathersSurname { get; set; }
        
        public required string MothersSurname { get; set; }
        
        public required string DocumentType { get; set; } // 'DNI' o 'CE'
        
        public required int DocumentNumber { get; set; } // 8-9 dígitos
        
        public required DateTime DateOfBirth { get; set; }
        
        public required string Nationality { get; set; } // Por defecto 'peruana' si es DNI
        
        public required string Gender { get; set; } // 'M' o 'F'
        
        public required string MainEmail { get; set; }
        
        public string? SecondaryEmail { get; set; }
        
        public required string MainPhone { get; set; } // 9-11 caracteres
        
        public string? SecondaryPhone { get; set; }
        
        public required string ContractType { get; set; }
        
        public required DateTime HiringDate { get; set; }
        
        public required byte[] UserPasswordEncrypt { get; set; }
        
        public required byte[] UserSalt { get; set; }
        
        public required int CVF { get; set; } // 0-5
        
        public required string ValidationCode { get; set; } // Random 12 dígitos
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
