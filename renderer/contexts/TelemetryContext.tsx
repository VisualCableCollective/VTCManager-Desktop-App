import React, {createContext, useContext, useEffect, useState} from "react";
import { TelemetryData } from "trucksim-telemetry";
import {useIpc} from "./IpcContext";

interface ContextProps {
    data: TelemetryData,
    active: boolean
}

const TelemetryContext = createContext<ContextProps>({
    data: null,
    active: false,
});

interface Props {
    children: React.ReactNode,
}
export function TelemetryContextProvider(props: Props) {
    const [data, setData] = useState<TelemetryData>(null);
    const [active, setActive] = useState<boolean>(false);
    const ipcRenderer = useIpc();
    useEffect(() => {
        if (!ipcRenderer.ipcReady){
            return;
        }

        ipcRenderer.ipcRenderer.on("telemetry", function(event, args) {
            console.log(args);
            if (args.active != active) {
                setActive(args.active);
            }

            setData(args.data);
        })
    }, [ipcRenderer.ipcReady]);

    const context = {
        data,
        active
    };

    return (
        <TelemetryContext.Provider value={context}>
            {props.children}
        </TelemetryContext.Provider>
    )
}

export const useTelemetry = () => useContext(TelemetryContext);