export class StoreJobRequest {
    // Cargo
    CargoId: string;
    CargoName: string;
    CargoMass: number;

    // Navigation Data
    PlannedDistanceKm: number;
    PlannedDistanceMiles: number;
    CitySourceId: string;
    CitySourceName: string;
    CompanySourceId: string;
    CompanySourceName: string;
    CityDestinationId: string;
    CityDestinationName: string;
    CompanyDestinationId: string;
    CompanyDestinationName: string;

    // Truck Data
    TruckModelId: string;
    TruckModelName: string;
    TruckModelManufacturerId: string;
    TruckModelManufacturerName: string;
    TruckCabinDamage: number;
    TruckChassisDamage: number;
    TruckEngineDamage: number;
    TruckTransmissionDamage: number;
    TruckWheelsDamage: number;

    // Trailer
    TrailerDamageChassis: number;
    TrailerDamageWheels: number;

    // Additional Data
    MarketId: string;
    IsSpecialJob: boolean;
    JobIngameStarted: string;
    JobIngameDeadline: string;d
    JobIncome: number;
    LanguageCode: string;

    private returnData = {};

    public GetPostData() {
        this.returnData = {};

        this.ValidateAndSetData("cargo_id", this.CargoId);
        this.ValidateAndSetData("cargo_name", this.CargoName);
        this.ValidateAndSetData("cargo_mass", this.CargoMass);

        this.ValidateAndSetData("planned_distance_km", this.PlannedDistanceKm);
        this.ValidateAndSetData("city_departure_id", this.CitySourceId);
        this.ValidateAndSetData("city_departure_name", this.CitySourceName);
        this.ValidateAndSetData("company_departure_id", this.CompanySourceId);
        this.ValidateAndSetData("company_departure_name", this.CompanySourceName);
        this.ValidateAndSetData("city_destination_id", this.CityDestinationId);
        this.ValidateAndSetData("city_destination_name", this.CityDestinationName);
        this.ValidateAndSetData("company_destination_id", this.CompanyDestinationId);
        this.ValidateAndSetData("company_destination_name", this.CompanyDestinationName);

        this.ValidateAndSetData("truck_model_id", this.TruckModelId);
        this.ValidateAndSetData("truck_model_name", this.TruckModelName);
        this.ValidateAndSetData("truck_model_manufacturer_id", this.TruckModelManufacturerId);
        this.ValidateAndSetData("truck_model_manufacturer_name", this.TruckModelManufacturerName);
        this.ValidateAndSetData("truck_cabin_damage_at_start", this.TruckCabinDamage);
        this.ValidateAndSetData("truck_chassis_damage_at_start", this.TruckChassisDamage);
        this.ValidateAndSetData("truck_engine_damage_at_start", this.TruckEngineDamage);
        this.ValidateAndSetData("truck_transmission_damage_at_start", this.TruckTransmissionDamage);
        this.ValidateAndSetData("truck_wheels_avg_damage_at_start", this.TruckWheelsDamage);

        this.ValidateAndSetData("trailer_avg_damage_chassis_at_start", this.TrailerDamageChassis);
        this.ValidateAndSetData("trailer_avg_damage_wheels_at_start", this.TrailerDamageWheels);

        this.ValidateAndSetData("market_id", this.MarketId);
        this.ValidateAndSetData("special_job", this.IsSpecialJob);
        this.ValidateAndSetData("job_ingame_started", this.JobIngameStarted);
        this.ValidateAndSetData("job_ingame_deadline", this.JobIngameDeadline);
        this.ValidateAndSetData("job_income", this.JobIncome);
        this.ValidateAndSetData("language_code", this.LanguageCode);

        return this.returnData;

    }

    public ValidateAndSetData(postDataKey: string, postDataValue: any) {
        this.returnData[postDataKey] = postDataValue;
    }
}