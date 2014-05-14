TRUNCATE TABLE CAT_MOVIMIENTO CASCADE;
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Descargado', true); -- 1
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Actualizado', true); -- 2
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Asignado', true); -- 3
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Escalado', true); -- 4
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Llamada', false); -- 5
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Visita', false); -- 6
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Email', false); -- 7
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Otro Seguimiento', false); -- 8
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Cerrado', true); -- 9
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Cancelado', true); -- 10
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Re-abierto', true); -- 11
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Deasignado', true); -- 12
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Desescalado', true); -- 13
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Incobrable', true); -- 14
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Recuperada', true); -- 15
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Atendida', true); -- 16
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Revisada', true); -- 17
