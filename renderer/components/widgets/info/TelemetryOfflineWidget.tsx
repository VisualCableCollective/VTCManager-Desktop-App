import {WidgetBase} from "../../shared/WidgetBase";
import ErrorAnimation from "../../../assets/lotties/error.json";
import Lottie from "lottie-react";
import React from "react";

export function TelemetryOfflineWidget() {
    return (
        <WidgetBase className="flex items-center justify-center">
            <div className="mx-6 max-w-md text-center">
                <div className="flex justify-center">
                    <div style={{maxWidth: "170px"}}>
                        <Lottie animationData={ErrorAnimation}  loop={false}  />
                    </div>
                </div>
                <h1 className="text-2xl font-bold">Telemetry Offline!</h1>
                <p className="text-sm mt-1">Oupsie, it looks like you aren't playing a game currently. If that's wrong, then try to reinstall the telemetry plugin in the settings.</p>
            </div>
        </WidgetBase>
    )
}