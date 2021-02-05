namespace UVNetGeneradorDeCodigo.Modelos
{
    using System.Collections.Generic;
    using UVNetGeneradorDeCodigo.Dialogos;

    public class CModificacionProceso
    {
        public string BaseDeDatos { get; set; }

        public string Esquema { get; set; }

        public string Proceso { get; set; }

        public List<Parametro> Columnas { get; set; }

        public OperacionModificacion OperacionPRE { get; set; }

        public OperacionModificacion OperacionEN { get; set; }

        public OperacionModificacion ProcedimientoObtencionPRE { get; set; }

        public OperacionModificacion ProcedimientoObtencionEN { get; set; }
    }

    public enum OperacionModificacion
    {
        SinCambios,
        Crear,
        Modificar,
        Eliminar
    }
}
