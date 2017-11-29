// Plan Quality metrics defined at ALCC
// fn-ALCC 2017
//
// Defined trying to make maximun use of VMS types
// Trying to make them generic for supporting cGy as treatment total dose unit (ALCC uses Gy)
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
    /// <summary>
    /// A class tha has methods returning plan metrics, when the metric implies a comparison V20% &lt 3cm3
    /// then a Tuple Metric,bool is returned, if not the metric is returned.
    /// </summary>
    public static class ALCC_QM
    {
        /// <summary>
        ///  V{Dose_Metric}[%,cm3] &lt Vol_Goal  Vol_repr=0 => Relative [%] Vol_repr=1 => Absolute [cm3]
        ///  using VolumePresentatio Type
        /// </summary>
        /// <param name="my_plan"></param>
        /// <param name="structure"></param>
        /// <param name="Dose_Metric"></param>
        /// <param name="Vol_Goal"></param>
        /// <param name="Vol_repr"></param>
        /// <returns></returns>
        public static Tuple<Double, Boolean> V_X_less_than(PlanningItem my_plan, Structure structure,
            DoseValue Dose_Metric, Double Vol_Goal, VolumePresentation Vol_repr)
        {
            Double Vol_Metric = my_plan.GetVolumeAtDose(structure, Dose_Metric, Vol_repr);
            Boolean test = (Vol_Metric <= Vol_Goal);
            return Tuple.Create(Vol_Metric, test);
        }

        //D{Vol_Metric [%,cm3]}[Gy,%] <= Dose_Metric [Unit of dose defined in Dose_Metric]
        // Volume units defined by Vol_repr=0 => Relative [%] Vol_repr=1 => Absolute [cm3] using VolumePresentation Type.

        public static Tuple<DoseValue, Boolean> D_X_less_than(PlanningItem my_plan, Structure structure,
            Double Vol_Metric, VolumePresentation Vol_repr, DoseValue Dose_Goal)
        {
            DoseValue Dose_Metric = new DoseValue(0.0, Dose_Goal.Unit);
            Boolean test = new Boolean();
            DoseValuePresentation Dose_present = DoseValuePresentation.Absolute;
            if (Dose_Goal.UnitAsString == "%")
            {
                Dose_present = DoseValuePresentation.Relative;
            }

            Dose_Metric = my_plan.GetDoseAtVolume(structure, Vol_Metric, Vol_repr, Dose_present);
            test = (Dose_Metric.Dose <= Dose_Goal.Dose);

            return Tuple.Create(Dose_Metric, test);
        }

        // Mean_Dose [Gy,%] units defined by Dose_repr=0 => Relative [%] Dose_repr=1 => Absolute [plan units]
        public static DoseValue Mean_Dose(PlanningItem my_plan, Structure structure, DoseValuePresentation Dose_present)
        {
            double bin_with = 0.01;
            return my_plan.GetDVHCumulativeData(structure, Dose_present,
                VolumePresentation.Relative, bin_with).MeanDose;
        }

        // Max [Gy,%] units defined by Dose_repr=0 => Relative [%] Dose_repr=1 => Absolute [plan units]
        public static DoseValue Max_Dose(PlanningItem my_plan, Structure structure, DoseValuePresentation Dose_present)
        {
            // Mean_Dose [Gy,%] units defined by Dose_repr=0 => Relative [%] Dose_repr=1 => Absolute [plan units]
            double bin_with = 0.01;
            return my_plan.GetDVHCumulativeData(structure, Dose_present,
                VolumePresentation.Relative, bin_with).MaxDose;
        }

        // Min [Gy,%] units defined by Dose_repr=0 => Relative [%] Dose_repr=1 => Absolute [plan units]
        public static DoseValue Min_Dose(PlanningItem my_plan, Structure structure, DoseValuePresentation Dose_present)
        {
            // Mean_Dose [Gy,%] units defined by Dose_repr=0 => Relative [%] Dose_repr=1 => Absolute [plan units]
            double bin_with = 0.01;
            return my_plan.GetDVHCumulativeData(structure, Dose_present,
                VolumePresentation.Relative, bin_with).MinDose;
        }

        // D{Vol[%,cm3]} [Gy] Volume units defined by Vol_repr=0 => Relative [%] Vol_repr=1 => Absolute [cm3]
        // using VolumePresentatio Type
        public static DoseValue D_X_report(PlanningItem my_plan, Structure structure, Double Vol_Metric,
            VolumePresentation Vol_repr, DoseValuePresentation Dose_present)
        {
            return my_plan.GetDoseAtVolume(structure, Vol_Metric, Vol_repr, Dose_present);
        }

        //V{Dose_Metric}[%,cm3] report:  Vol_repr=0 => Relative [%] Vol_repr=1 => Absolute [cm3]
        // using VolumePresentatio Type
        public static Double V_X_report(PlanningItem my_plan, Structure structure, DoseValue Dose_Metric, VolumePresentation Vol_repr)
        {
            return my_plan.GetVolumeAtDose(structure, Dose_Metric, Vol_repr);
        }

        //CI report: CI=BODY(V100%Dprc)/PTV(Vol)
        public static Double CI(PlanningItem my_plan, Structure ptv,Structure body, DoseValue Dose_presc)
        {
            double BV100 = my_plan.GetVolumeAtDose(body, Dose_presc, VolumePresentation.AbsoluteCm3);
            double PTVvol = ptv.Volume;

            return BV100/PTVvol;
        }

        //HI report: HI=(D2%-D98%)/D50% 
        public static Double HI(PlanningItem my_plan, Structure ptv)
        {
            double d2 = my_plan.GetDoseAtVolume(ptv,2.0,VolumePresentation.Relative,DoseValuePresentation.Absolute).Dose;
            double d98 = my_plan.GetDoseAtVolume(ptv, 98, VolumePresentation.Relative, DoseValuePresentation.Absolute).Dose;
            double d50= my_plan.GetDoseAtVolume(ptv, 50.0, VolumePresentation.Relative, DoseValuePresentation.Absolute).Dose;

            return (d2-d98)/d50;
        }


    }
}
