import {WidgetBase} from "../../shared/WidgetBase";
import ConnectedAnimation from "../../../assets/lotties/telemetry-connected.json";
import Lottie from "lottie-react";
import React from "react";

export function TelemetryOnlineWidget() {
    return (
        <WidgetBase className="flex items-center justify-center">
            <div className="mx-6 max-w-md text-center">
                <div className="flex justify-center">
                    <div style={{maxWidth: "170px"}}>
                        <Lottie animationData={ConnectedAnimation}  loop={false}  />
                    </div>
                </div>
                <h1 className="text-2xl font-bold">Telemetry Online!</h1>
                <p className="text-sm mt-1"></p>
            </div>
        </WidgetBase>
    )
}