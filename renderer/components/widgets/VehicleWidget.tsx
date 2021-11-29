import { WidgetBase } from "../shared/WidgetBase";

// icons
import { FaGasPump } from "react-icons/fa";
import { WiDayRain } from "react-icons/wi";
import {useTelemetry} from "../../contexts/TelemetryContext";

export function VehicleWidget() {
  const telemetry = useTelemetry();

  return (
    <div className="flex flex-col justify-between w-1/3">
      <div className="w-full">
        <div>
          <h1 className="font-bold text-3xl text-center">{telemetry.data ? telemetry.data.truck.speed.kph : "n/a"}</h1>
          <h2 className="text-opacity-80 text-white text-center">Km/h</h2>
        </div>
      </div>
      <div className="flex justify-center items-center h-8 w-full">
        <div className="text-2xl mr-6">
          <FaGasPump />
        </div>
        <div className="flex items-center w-52">
          <div className="bg-blue-900 rounded-xl h-10 absolute w-52"></div>
          <div
            className="bg-blue-500 rounded-xl h-10 z-10"
            style={{ width: telemetry.data ? (telemetry.data.truck.fuel.value / telemetry.data.truck.fuel.capacity) + "%" : "0%" }}
          ></div>
          <h1 className="z-20 text-center w-52 absolute font-semibold">{telemetry.data ? (telemetry.data.truck.fuel.value / telemetry.data.truck.fuel.capacity) + "%" : "0%"}</h1>
        </div>
        <p className="text-white text-opacity-80 ml-6 text-center leading-4 text-sm">{telemetry.data ? telemetry.data.navigation.distance + "km" : "n/a"}<br/>Distance</p>
      </div>
    </div>
  );
}