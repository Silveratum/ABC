using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ABCWeb.Models
{
  public class ServiceRequest
  {
    public int ServiceId { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Mail { get; set; }

    [Required]
    public int ServiceType { get; set; }

    [Required]
    public float Price { get; set; }

    public ServiceStatus Status { get; set; }

    public enum ServiceStatus
    {
      Sent = 1,
      Approved,
      Rejected,
      InProgress,
      WaitingPayment,
      Closed
    }
  }
}
