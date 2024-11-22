﻿using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class PaymentRepository : IPayment
    {
        private readonly DentalManagementDbContext _context;

        public PaymentRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddPaymentAsync(Payment payment)
        {
             _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId)
        {
           return await _context.Payments.Where(p=>p.InvoiceId == invoiceId).ToListAsync();
        }

        public int Count()
        {
            return _context.Payments.Count();
        }
    }
}
