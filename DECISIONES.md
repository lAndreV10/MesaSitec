# Decisiones del proyecto

## Decisiones técnicas

1. Usé SQLite con EF Core porque es la base de datos indicada en el enunciado y permite levantar el proyecto sin instalar servicios adicionales. Descarté usar SQL Server o una base de datos en la nube porque añadirían configuración que no era necesaria.

2. Separé el backend en Api, Aplicacion, Dominio e Infraestructura. La alternativa era colocar toda la lógica en los endpoints, pero preferí dejar las reglas de estados, permisos y SLA en Dominio para poder probarlas sin levantar la API.

3. En el frontend usé Pinia para conservar la sesión y un cliente HTTP único para enviar el token JWT. La alternativa era repetir la configuración de `fetch` en cada vista, pero eso aumentaba el riesgo de olvidar el token o el manejo de un error 401.

## Uso de IA

Usé IA como guía de aprendizaje durante el proyecto. La consulté para entender el enunciado, modelar las relaciones, aclarar conceptos de .NET y Vue, consultar ejemplos en partes que estaba aprendiendo y revisar errores de compilación.

No tomé las respuestas como resultado final sin revisarlas. Cada bloque se integró de forma progresiva, se compiló y se probó. También ejecuté las pruebas unitarias y recorrí manualmente los flujos de crear, editar y cambiar estados. La responsabilidad de entender el código, las decisiones y cualquier cambio durante una entrevista es mía.

## Si tuviera una semana más

Agregaría pruebas de integración para los endpoints y mejoraría la selección de agentes con un endpoint específico, siempre que el contrato de la prueba lo permitiera. También dedicaría más tiempo a revisar los mensajes de error y la experiencia visual.

## Dificultad encontrada

Me atasqué al iniciar la API varias veces porque había otro proceso ejecutándose y bloqueaba los archivos de compilación. Lo resolví identificando el proceso que usaba el puerto 5080, cerrándolo y dejando una sola ejecución de la API activa.
