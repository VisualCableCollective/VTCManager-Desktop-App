import Telemetry, {EventsJobStartedVerbose, TelemetryData, Telemetry as TelemetryType} from "trucksim-telemetry";
import Log from "@awesomeeng/awesome-log";
import {BrowserWindow} from "electron";
import {StoreJobRequest} from "../../modules/vtcm-api-client/models/requests/StoreJobRequest";
import {VtcmApiClient} from "../../modules/vtcm-api-client";
import {Storage} from "./StorageManager";

export class TelemetryManager {
    static telemetry: TelemetryType;
    static readonly UpdateInterval = 50;
    static appWindow : BrowserWindow;

    static Init(mainWindow: BrowserWindow) {
        TelemetryManager.appWindow = mainWindow;
        TelemetryManager.telemetry = Telemetry();

        TelemetryManager.UpdateReceived(null);
        TelemetryManager.telemetry.job.on("started", this.JobStarted);

        TelemetryManager.telemetry.watch({interval: TelemetryManager.UpdateInterval}, TelemetryManager.UpdateReceived);
        Log.info("[Telemetry] Listening to telemetry updates with an interval of " + TelemetryManager.UpdateInterval);
    }

    static UpdateReceived(data: TelemetryData) {
        let active = true;
        if (data === null) {
            active = false;
        }

        TelemetryManager.appWindow.webContents.send("telemetry", {
            active,
            data
        });
    }

    static JobStarted(data: EventsJobStartedVerbose) {
        Log.info("[Telemetry] A new job has been started!");
        Log.debug("[Telemetry] Job Data: " + JSON.stringify(data));

        const requestData = new StoreJobRequest();
        requestData.CargoId = data.cargo.id;
        requestData.CargoName = data.cargo.name;
        requestData.CargoMass = data.cargo.mass;

        requestData.PlannedDistanceKm = data.plannedDistance.km;
        requestData.PlannedDistanceMiles = data.plannedDistance.miles;
        requestData.CitySourceId = data.source.city.id;
        requestData.CitySourceName = data.source.city.name;
        requestData.CompanySourceId = data.source.company.id;
        requestData.CompanySourceName = data.source.company.name;
        requestData.CityDestinationId = data.destination.city.id;
        requestData.CityDestinationName = data.destination.city.name;
        requestData.CompanyDestinationId = data.destination.company.id;
        requestData.CompanyDestinationName = data.destination.company.name;

        const truckData = TelemetryManager.telemetry.getTruck();
        requestData.TruckModelId = truckData.model.id;
        requestData.TruckModelName = truckData.model.name;
        requestData.TruckModelManufacturerId = truckData.make.id;
        requestData.TruckModelManufacturerName = truckData.make.name;
        requestData.TruckCabinDamage = truckData.damage.cabin;
        requestData.TruckChassisDamage = truckData.damage.chassis;
        requestData.TruckEngineDamage = truckData.damage.engine;
        requestData.TruckTransmissionDamage = truckData.damage.transmission;
        requestData.TruckWheelsDamage = truckData.damage.wheels;

        const trailerData = TelemetryManager.telemetry.getTrailer();
        requestData.TrailerDamageChassis = trailerData.damage.chassis;
        requestData.TrailerDamageWheels = trailerData.damage.wheels;

        requestData.MarketId = data.market.id;
        requestData.IsSpecialJob = data.isSpecial;
        // ToDo: I hate this shit
        requestData.JobIngameStarted = new Date(TelemetryManager.telemetry.getData().game.time.unix).toISOString();
        requestData.JobIngameDeadline = new Date(data.expectedDeliveryTimestamp.unix).toISOString();
        requestData.JobIncome = data.income;
        requestData.LanguageCode = null;

        VtcmApiClient.JobStart(requestData.GetPostData()).then((response) => {
            if (response.success) {
                Storage.set("CurrentJobId", response.id);
                console.log("Started job: " + response.id);
            } else {
                console.log("Failed to start job");
            }
        })
    }
}