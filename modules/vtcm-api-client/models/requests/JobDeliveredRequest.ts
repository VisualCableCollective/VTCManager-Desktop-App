export class JobDeliveredRequest {
    // Job Data
    JobId: number;
    RemainingDeliveryTime: string;
    RemainingDistance: string;

    // Cargo Data
    CargoDamage: number;

    // Truck Data
    TruckCabinDamage: number;
    TruckChassisDamage: number;
    TruckEngineDamage: number;
    TruckTransmissionDamage: number;
    TruckWheelsDamage: number;

    // Trailer Data
    TrailerChassisDamage: number;
    TrailerWheelsDamage: number;

    private returnData = {};

    public GetPostData() {
        this.returnData = {};

        this.ValidateAndSetData("job_id", this.JobId);
        this.ValidateAndSetData("remaining_delivery_time", this.RemainingDeliveryTime);
        this.ValidateAndSetData("remaining_distance", this.RemainingDistance);

        this.ValidateAndSetData("cargo_damage", this.CargoDamage);

        this.ValidateAndSetData("truck_cabin_damage_at_end", this.TruckCabinDamage);
        this.ValidateAndSetData("truck_chassis_damage_at_end", this.TruckChassisDamage);
        this.ValidateAndSetData("truck_engine_damage_at_end", this.TruckEngineDamage);
        this.ValidateAndSetData("truck_transmission_damage_at_end", this.TruckTransmissionDamage);
        this.ValidateAndSetData("truck_wheels_avg_damage_at_end", this.TruckWheelsDamage);

        this.ValidateAndSetData("trailer_avg_damage_chassis_at_end", this.TrailerChassisDamage);
        this.ValidateAndSetData("trailer_avg_damage_wheels_at_end", this.TrailerWheelsDamage);

        return this.returnData;
    }

    public ValidateAndSetData(postDataKey: string, postDataValue: any) {
        this.returnData[postDataKey] = postDataValue;
    }
}