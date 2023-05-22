using System.ComponentModel;

namespace PayBill.Core.Constants;

public enum UtilityCompany
{
    [Description("DESCO Postpaid")] DhakaElectricSupplyCompanyPostpaid = 1,
    [Description("Khulna Wasa")] KhulnaWasa,
    [Description("DPDC")] DhakaPowerDistributionCompany,
    [Description("BTCL")] BangladeshTelecommunicationsCompanyLimited,
    [Description("JGTCL")] JalalabadGasTransmissionLimited,
    [Description("Dhaka Wasa")] DhakaWasa,
    [Description("Bakhrabad Gas")] BakhrabadGas,
    [Description("WZPDCL")] WestZonePowerDistribution,
    [Description("e-Porcha")] EPorcha,
    [Description("NESCO")] NorthernElectricitySupplyCompanyLtd,
    [Description("Rajshahi Wasa")] RajshahiWasa,
    [Description("Land Tax")] LandTax,
    [Description("DESCO Prepaid")] DhakaElectricSupplyCompanyPrepaid,
    [Description("PGCL")] PashchimanchalGasCompanyLimited,
    [Description("e-Mutation")] EMutation,
    [Description("NSDA")] NationalSkillsDevelopmentAuthority
}