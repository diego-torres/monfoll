TRUNCATE TABLE CAT_MOVIMIENTO CASCADE;
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Descargado', true); -- 1
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Actualizado', true); -- 2
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Lista Negra', true); -- 3
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Atendida', true); -- 4
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Revisada', true); -- 5
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Escalada', true); -- 6
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Incobrable', true); -- 7
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Recuperada', true); -- 8
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Llamada', false); -- 9
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Visita', false); -- 10
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Email', false); -- 11
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Otro Seguimiento', false); -- 12
INSERT INTO CAT_MOVIMIENTO(descripcion, system_based) VALUES('Registro', true); -- 13