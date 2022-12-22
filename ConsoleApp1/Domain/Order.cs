using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCurriersSchedulerStudyApp.Domain
{
    internal class Order
    {
        public Location FromLocation { get; set; }

        public Location ToLocation { get; set; }

        public double Weigth { get; set; }

        public double OrderDistance
        {
            get { return FromLocation.GetDistance(ToLocation); }
        }

        public double OrderPrice
        {
            get
            {
                return GetOrderPrice();
            }
        }

        public PlanningOption CurrentPlan { get; private set; }

        public bool IsPlanned { get { return CurrentPlan != null; } }

        private double GetOrderPrice()
        {
            return OrderDistance * Company.PricePerDistance;
        }

        public string GetInfo()
        {
            return $"Заказ {FromLocation.ToString()} -> {ToLocation.ToString()}" +
                $" ({OrderDistance} км) | {OrderPrice}";
        }

        List<PlanningOption> _planningOptions = new List<PlanningOption>();

        public bool PlanOrderAction()
        {
            var curriers = FindCurriers();

            var planningOptions = new List<PlanningOption>();

            foreach (var currier in curriers)
            {
                var planningOption = currier.RequestPlanningOptionAction(this);

                if (planningOption != null)
                {
                    planningOptions.Add(planningOption);
                }
            }

            if (planningOptions.Count() > 0)
            {
                var bestOption = GetBestOption(planningOptions);

                if (bestOption != null)
                {
                    bestOption.Curier.AcceptPlanAction(bestOption);
                    CurrentPlan = bestOption;

                    return true;
                }
            }

            return false;
        }
     
        private IList<Curier> FindCurriers()
        {
            var curriers = Company.CompanyInstance.GetAvailibleCurriers()
                                .Where(x => x.CanCarry(this));

            return curriers.ToList();
        }

        private PlanningOption GetBestOption(IList<PlanningOption> options)
        {
            var sortedOption = options.OrderByDescending(option => option.Profit);
            var bestOption = sortedOption.FirstOrDefault(bestOption => bestOption.Profit > 0);

            return bestOption;
        }

        internal void ReviewOffer(PlanningOption option)
        {
            if (!IsPlanned || CurrentPlan.Profit < option.Profit)
            {
                Console.WriteLine($"Заказ получил выгодное предложение {option.Profit}" +
                    $" от курьера {option.Curier.Name}");

                option.Curier.AcceptPlanAction(option);
                CurrentPlan = option;
            }
        }
    }
}
