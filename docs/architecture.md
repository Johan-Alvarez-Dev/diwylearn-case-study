# Arquitectura pública

DiwyLearn combina capas técnicas con módulos verticales por capacidad.

- `Domain`: cursos, inscripciones y progreso.
- `Data`: DbContext, mappings y migraciones.
- `Features`: contratos, controllers y services por módulo.
- `Infrastructure`: JWT, Identity y sanitización.
- `Common`: roles y errores compartidos.

Las entidades EF no se exponen como contratos HTTP. Los resúmenes se proyectan en SQL. Identity administra hash y lockout; JWT transporta identidad y roles; editar exige rol y propiedad salvo administración.

El editor y el player comparten shells y parsers para evitar divergencia. La estructura deja espacio para tenancy, pero no lo presenta como implementado.
