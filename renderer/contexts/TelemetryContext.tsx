import React, {createContext, useContext, useEffect, useState} from "react";
import { TelemetryData } from "trucksim-telemetry";
import {useIpc} from "./IpcContext";

interface ContextProps {
    data: TelemetryData
}

const TelemetryContext = createContext<ContextProps>({
    data: null
});

interface Props {
    children: React.ReactNode,
}
export function TelemetryContextProvider(props: Props) {
    const [data, setData] = useState<TelemetryData>(null);
    const ipcRenderer = useIpc();
    useEffect(() => {
        ipcRenderer.ipcRenderer.on("telemetry", function(event, args) {
            console.log("Received telemetry data: " + args);
            setData(args);
        })
    }, []);

    const context = {
        data,
    };

    return (
        <TelemetryContext.Provider value={context}>
            {props.children}
        </TelemetryContext.Provider>
    )
}

export const useTelemetry = () => useContext(TelemetryContext);